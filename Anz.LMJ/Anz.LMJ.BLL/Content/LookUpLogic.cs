using Anz.LMJ.BLO.ContentObjects;
using Anz.LMJ.BLO.LookUpObjects;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using FastMember;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Anz.LMJ.BLL.Content
{
    public class LookUpLogic
    {

        
        #region Accessors

        LookUpAccessor _LookUpAccessor = new LookUpAccessor();
        LookUpMultiLanguageAccessor _LookUpMultiAccessor = new LookUpMultiLanguageAccessor();
        LookUpMediaAccessor _MediaAccessor = new LookUpMediaAccessor();
        LanguageAccessor _LanguageAccessor=new LanguageAccessor();

        #endregion



        #region Static Shared Variables
        private static Dictionary<AdminTables, long> __SharedTableIds = new Dictionary<AdminTables, long>();
        private static Dictionary<AdminTables, System.Type> __SharedClassesForeachTable = new Dictionary<AdminTables, System.Type>();
        #endregion

        #region Constructors
        public LookUpLogic()
        {
            if (__SharedTableIds.Count() == 0)
            {
                __SharedTableIds[AdminTables.Hero_Banner] = 3;
                __SharedTableIds[AdminTables.About_Page] = 4;
                __SharedTableIds[AdminTables.Events] = 5;
                __SharedTableIds[AdminTables.EditorialBoard] = 6;
                __SharedTableIds[AdminTables.Videos] = 7; 
                __SharedTableIds[AdminTables.Team] = 8;
                __SharedTableIds[AdminTables.News] = 9;
                __SharedTableIds[AdminTables.IssueFilter] = 10;
                __SharedTableIds[AdminTables.Degree] = 11;
                __SharedTableIds[AdminTables.Position] = 12;
                __SharedTableIds[AdminTables.Contact] = 13;
                __SharedTableIds[AdminTables.FooterMenu] = 14;
                __SharedTableIds[AdminTables.Citation] = 17;
                __SharedTableIds[AdminTables.Index] = 18;
                __SharedTableIds[AdminTables.IndexType] = 19;
                __SharedTableIds[AdminTables.Footer] = 1010;
                

            }

            if (__SharedClassesForeachTable.Count() == 0)
            {
                
                __SharedClassesForeachTable[AdminTables.Hero_Banner] = typeof(Hero_Banner);
                __SharedClassesForeachTable[AdminTables.About_Page] = typeof(About_Page);
                __SharedClassesForeachTable[AdminTables.Events] = typeof(Events);
                __SharedClassesForeachTable[AdminTables.EditorialBoard] = typeof(EditorialBoard);
                __SharedClassesForeachTable[AdminTables.Videos] = typeof(Videos);
                __SharedClassesForeachTable[AdminTables.Team] = typeof(Team);
                __SharedClassesForeachTable[AdminTables.News] = typeof(News);
                __SharedClassesForeachTable[AdminTables.IssueFilter] = typeof(IssueFilter);
                __SharedClassesForeachTable[AdminTables.Degree] = typeof(DataType);
                __SharedClassesForeachTable[AdminTables.Position] = typeof(DataType);
                __SharedClassesForeachTable[AdminTables.Contact] = typeof(Contact);
                __SharedClassesForeachTable[AdminTables.FooterMenu] = typeof(FooterMenu);
                __SharedClassesForeachTable[AdminTables.Citation] = typeof(CitationType);
                __SharedClassesForeachTable[AdminTables.Index] = typeof(Index);
                __SharedClassesForeachTable[AdminTables.IndexType] = typeof(DataType);
                __SharedClassesForeachTable[AdminTables.Footer] = typeof(Footer);
            }





        }
        #endregion



        #region My Variables 
        public enum AdminTables
        {
            Hero_Banner,
            About_Page,
            Events,
            EditorialBoard,
            Videos,
            Team,
            News,
            EditorsPick,
            IssueFilter,
            Contact,
            Degree,
            Position,
            FooterMenu,
            CopyRight,
            Citation,
            Index,
            IndexType,
            Footer
        };

        #endregion

  


        #region Content


        public void DeleteContent(long parentId)
        {
            try
            {
                //delete all lookups that hold this id as parent or original one.
                _LookUpAccessor.DeleteContent(true, parentId);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public long AddContent<T>(T obj, AdminTables table)
        {
            string XMLPath = ConfigurationManager.AppSettings["XMLPath"];

            //string xmlPath = @"C:\Users\huda\Desktop\Anzimaty\Anz.LMJ\Anz.LMJ.StartUp\LookUp.xml";
            string multiLangValue;
            string imageValue;

            LookUpMedia img = new LookUpMedia();

            List<string> imageValues = new List<string>();


            List<LookUpAttributes> attributes = Tools.GetAttributes(table.ToString(), XMLPath);


            var t = typeof(T);


            LookUp lk = new LookUp();
            LookUpMultiLanguage ml = new LookUpMultiLanguage();
            long parentId = 0;
            bool isFirst = true;

            foreach (LookUpAttributes att in attributes)
            {


                lk = new LookUp();

                lk.TableId = (int)__SharedTableIds[table];
                lk.SysDate = DateTime.Now;
                lk.UserId = 1;
                lk.IsDeleted = false;
                lk.Code = att.Code;
                lk.isPublished = true;


                if (isFirst)
                {
                    lk.ParentId = null;

                }
                else
                {
                    lk.ParentId = parentId;
                }

                lk = _LookUpAccessor.Add(lk);

                if (isFirst)
                {
                    isFirst = false;
                    parentId = lk.Id;
                }


                if (att.isMedia)
                {
                    //check if the property is list
                    if (att.isList)
                    {
                        imageValues = (List<string>)(t.GetProperty(att.Name).GetValue(obj));

                        foreach (string s in imageValues)
                        {
                            // add media
                            img = new LookUpMedia();

                            img.isActive = true;
                            img.isDeleted = false;
                            img.isVideo = att.isVideo;
                            img.LookUpId = lk.Id; ;
                            img.SysDate = DateTime.Now;
                            img.Name = s;


                            _MediaAccessor.Add(img);
                        }
                    }
                    else
                    {
                        imageValue = (string)(t.GetProperty(att.Name).GetValue(obj));

                        img = new LookUpMedia();

                        img.isActive = true;
                        img.isDeleted = false;
                        img.isVideo = att.isVideo;
                        img.LookUpId = lk.Id;
                        img.SysDate = DateTime.Now;
                        img.Name = imageValue;


                        _MediaAccessor.Add(img);
                    }
                }
                else
                {
                    // add lookupmulti

                    multiLangValue = (t.GetProperty(att.Name).GetValue(obj)).ToString();

                    ml = new LookUpMultiLanguage();

                    ml.LookUpId = lk.Id;
                    if (att.isLangNull)
                    {
                        ml.LangId = null;
                    }
                    else
                    {
                        ml.LangId = 1;
                    }

                    ml.SysDate = DateTime.Now;
                    ml.isDeleted = false;
                    //ml.UserId = 1;
                    ml.Description = multiLangValue;
                   // ml.isPublished = true;

                    _LookUpMultiAccessor.Add(ml);
                }





            }
            return parentId;
        }


        public List<LookUp> GetRowsOfTableByCode(long tableId, string code)
        {
            List<LookUp> lookups = new List<LookUp>();

            try
            {
                lookups = _LookUpAccessor.GetList(tableId, code);
            }
            catch (Exception ex)
            {

                throw;
            }

            return lookups;
        }


        public List<T> ReturnListOf<T>(AdminTables table, long langId, int offset, int limit, List<LookUpAttributes> attributes, LookUpAttributes main, List<string> fields, bool isAll, out int total, long? lookUpId)
        {
            try
            {

                LookUpMultiLanguage lookupMulti = new LookUpMultiLanguage();
                LookUp lookUp = new LookUp();
                long tableId;
                List<LookUp> allRowsThatHaveCodeOfMainChild = new List<LookUp>();
                List<LookUp> selectedRowsThatHaveCodeOfMainChild = new List<LookUp>();
                List<LookUp> contents = new List<LookUp>();

                System.Type UsedClass = __SharedClassesForeachTable[table];
                var UsedObject = Activator.CreateInstance(UsedClass);

                List<string> images = new List<string>();

                dynamic dynamic = (dynamic)UsedObject;
                tableId = __SharedTableIds[table];
                List<dynamic> allDynamic = new List<dynamic>();


                var accessor = FastMember.TypeAccessor.Create(typeof(T));

                if (lookUpId == null)
                {
                    allRowsThatHaveCodeOfMainChild = GetRowsOfTableByCode(tableId, main.Code);

                    total = allRowsThatHaveCodeOfMainChild.Count();
                    selectedRowsThatHaveCodeOfMainChild = allRowsThatHaveCodeOfMainChild.OrderByDescending(s => s.Id).Skip(offset).Take(limit).ToList();
                }
                else
                {
                    selectedRowsThatHaveCodeOfMainChild.Add(_LookUpAccessor.Get((long)lookUpId));
                    total = 1;
                }

                foreach (LookUp row in selectedRowsThatHaveCodeOfMainChild)
                {
                    contents = new List<LookUp>();
                    contents = _LookUpAccessor.GetChildren(row.Id);
                    dynamic = (dynamic)Activator.CreateInstance(UsedClass);
                    accessor[dynamic, "Id"] = row.Id;

                    // main
                    if (main.isLangNull)
                        lookupMulti = _LookUpMultiAccessor.Get(row.Id, null);
                    else
                        lookupMulti = _LookUpMultiAccessor.Get(row.Id, langId);
                    if (lookupMulti != null)
                        accessor[dynamic, main.Name] = lookupMulti.Description;

                    //TO DO set id 

                    foreach (LookUpAttributes obj in attributes)
                    {

                        if (obj.Code == main.Code)
                            continue;
                        if (!isAll)
                        {
                            if (fields.Contains(obj.Name) == false)
                                continue;
                        }
                        lookUp = new LookUp();
                        lookUp = contents.Where(e => e.Code == obj.Code).FirstOrDefault();
                        if (lookUp == null)
                            continue;
                        if (obj.Name == "Date")
                        {
                            accessor[dynamic, obj.Name] = (row.SysDate).Value.Date; //new
                            continue;
                        }
                        if (obj.isMedia)
                        {
                            try
                            {
                                if (obj.isList)
                                {

                                    //get list
                                    images = new List<string>();
                                    images = _MediaAccessor.Get(lookUp.Id, obj.isVideo)
                                        .Select(e => e.Name).ToList();
                                    if (images.Count != 0)
                                        accessor[dynamic, obj.Name] = images;
                                }
                                else
                                {
                                    LookUpMedia media = new LookUpMedia();
                                    media = _MediaAccessor.Get(lookUp.Id, obj.isVideo).FirstOrDefault();
                                    if (media == null)
                                        continue;
                                    // get one image
                                    accessor[dynamic, obj.Name] = media.Name;
                                }
                            }
                            catch (Exception ex)
                            {

                                throw new Exception("exception in media id=" + lookUp.Id);
                            }

                        }
                        else
                        {
                            try
                            {
                                if (obj.isLangNull)
                                    lookupMulti = _LookUpMultiAccessor.Get(lookUp.Id, null);
                                else
                                    lookupMulti = _LookUpMultiAccessor.Get(lookUp.Id, langId);

                                accessor[dynamic, obj.Name] = lookupMulti.Description;
                            }
                            catch (Exception ex)
                            {

                                throw new Exception("exception in lookup multi id=" + lookUp.Id);
                            }
                        }


                    }
                    allDynamic.Add(dynamic);

                }

                List<T> list = new List<T>();
                foreach (dynamic d in allDynamic)
                {
                    list.Add((T)d);
                }


                return list;


            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<LookUpAttributes> GetAttributes(AdminTables table,string langCode)
        {
            LookUpAttributes main = new LookUpAttributes();
            Language lang = new Language();
            List<LookUpAttributes> attributes = new List<LookUpAttributes>();

            try
            {
                string XMLPath = ConfigurationManager.AppSettings["XMLPath"]; // @"~\LookUp.xml";

                lang = _LanguageAccessor.Get(langCode);

                //Get class's attributes

                attributes = Tools.GetAttributes(table.ToString(), XMLPath);

                return attributes;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public GeneralContents<T> GetContent<T>(AdminTables table, string langCode, int offset, int limit, List<string> fields, bool isAll, long? LookUpId)
        {
            GeneralContents<T> contents = new GeneralContents<T>();

            LookUpAttributes main = new LookUpAttributes();
            Language lang = new Language();
            List<LookUpAttributes> attributes = new List<LookUpAttributes>();

            try
            {
                string XMLPath = ConfigurationManager.AppSettings["XMLPath"]; // @"~\LookUp.xml";
               
                lang = _LanguageAccessor.Get(langCode);

                //Get class's attributes
                
                attributes = Tools.GetAttributes(table.ToString(), XMLPath);
                //Get main attribute
                main = attributes
                       .Where(e => e.isMain == true)
                       .FirstOrDefault();

                int total = 0;
                if (main == null)
                {
                    main = new LookUpAttributes();
                    main.Code = "This is any text";
                }

                /* contents = GetMono<T>(table, lang.Id, main.Code);*/
                if (attributes.Count != 0)
                {
                    contents.Contents = ReturnListOf<T>(table, lang.Id, offset, limit, attributes, main, fields, isAll, out total, LookUpId);
                }
                contents.TotalContent = total;
            }
            catch (Exception ex)
            {
                throw;
            }
            return contents;
        }
        #endregion



        public bool UpdatePosition(string positionCode, long tableId, bool isAdd, long? recordId)
        {
            try
            {

                //delete
                if (!isAdd)
                {
                   
                    //get position
                    LookUp position = _LookUpAccessor.Get(positionCode, (long)recordId, false);
                    LookUpMultiLanguage multi = new LookUpMultiLanguage();
                    LookUpMultiLanguage posMulti = new LookUpMultiLanguage();
                    posMulti = _LookUpMultiAccessor.Get(position.Id, 1);
                    List<LookUp> positions = _LookUpAccessor.GetList(tableId).Where(e => e.Code == positionCode && e.Id != position.Id && e.IsDeleted == false).ToList();

                    foreach (LookUp p in positions)
                    {
                        //get multi
                        multi = new LookUpMultiLanguage();
                        multi = _LookUpMultiAccessor.Get(p.Id, 1);

                        if (long.Parse(multi.Description) >= long.Parse(posMulti.Description))
                        {
                            //update
                            _LookUpMultiAccessor.EditDesription(multi.Id, (long.Parse(multi.Description) - 1).ToString());
                        }
                    }
                }
                else
                {
                    List<LookUp> positions = _LookUpAccessor.GetList(tableId, positionCode);
                    LookUpMultiLanguage multi = new LookUpMultiLanguage();

                    foreach (LookUp p in positions)
                    {
                        //get multi
                        multi = new LookUpMultiLanguage();
                        if (tableId == 26)
                        {
                            multi = _LookUpMultiAccessor.Get(p.Id, null);

                        }
                        else
                        {
                            multi = _LookUpMultiAccessor.Get(p.Id, 1);

                        }

                        _LookUpMultiAccessor.EditDesription(multi.Id, (long.Parse(multi.Description) + 1).ToString());

                    }

                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }


    }
}
