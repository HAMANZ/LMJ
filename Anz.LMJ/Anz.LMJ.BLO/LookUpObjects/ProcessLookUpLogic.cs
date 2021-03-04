using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anz.LMJ.BLO.LookUpObjects
{
    public class ProcessLookUpLogic
    {
        int countryTableId = 1;
        int cityTableId = 2;

        #region Address

        #region Add
        public DynamicResponse<bool> AddCountriesAndCities()
        {
            DynamicResponse<bool> response = new DynamicResponse<bool>();
            try
            {
                ExcelAccessor _ExcelAccessor = new ExcelAccessor();
                //List<ExcelCountriesAndCity> data = _ExcelAccessor.GetList();

                CountryAccessor _CountryAccessor = new CountryAccessor();
                CityAccessor _CityAccessor = new CityAccessor();
                LookUpAccessor _LookUpAccessor = new LookUpAccessor();
                LookUpMultiLanguageAccessor _LookUpMultiLanguageAccessor = new LookUpMultiLanguageAccessor();

                Country country = new Country();
                City city = new City();
                LookUp lookUp = new LookUp();
                LookUpMultiLanguage lookUpMulti = new LookUpMultiLanguage();

                //foreach (ExcelCountriesAndCity item in data)
                //{
                //    country = new Country();
                //    country = _CountryAccessor.Get(item.iso3);

                //    //add country
                //    if (country == null)
                //    {
                //        country = _CountryAccessor.Add(item.iso3);

                //        lookUp = new LookUp();
                //        lookUp = _LookUpAccessor.Add(item.iso3, countryTableId, 1);

                //        lookUpMulti = new LookUpMultiLanguage();
                //        lookUpMulti = _LookUpMultiLanguageAccessor.Add(item.country, lookUp.Id, 1, 1);
                //    }

                    //add city
                //    city = new City();
                //    city = _CityAccessor.Add(item.city_ascii.ToUpper(), country.Id);

                //    lookUp = new LookUp();
                //    lookUp = _LookUpAccessor.Add(item.city_ascii.ToUpper(), cityTableId, 1);

                //    lookUpMulti = new LookUpMultiLanguage();
                //    lookUpMulti = _LookUpMultiLanguageAccessor.Add(item.city_ascii, lookUp.Id, 1, 1);


                //}
                 
                response.Data = true;
                response.HttpStatusCode = System.Net.HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ServerMessage = ex.Message;
                return response;
            }
        }
        #endregion

        #region Get

        public DynamicResponse<List<ReturnedObject>> GetCountries()
        {
            DynamicResponse<List<ReturnedObject>> response = new DynamicResponse<List<ReturnedObject>>();
            try
            {
                List<ReturnedObject> data = new List<ReturnedObject>();

                CountryAccessor _CountryAccessor = new CountryAccessor();
                List<Country> countries = _CountryAccessor.GetList();

                LookUp lookUp = new LookUp();
                LookUpMultiLanguage lookUpMulti = new LookUpMultiLanguage();

                LookUpAccessor _LookUpAccessor = new LookUpAccessor();
                LookUpMultiLanguageAccessor _LookUpMultiLanguageAccessor = new LookUpMultiLanguageAccessor();

                foreach (Country item in countries)
                {
                    lookUp = new LookUp();
                    lookUp = _LookUpAccessor.Get(item.Code, countryTableId);

                    if (lookUp == null)
                    {
                        response.Message = "please try again later";
                        response.ServerMessage = "lookup is null for country code: " + item.Code;
                        response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;

                        return response;
                    }

                    lookUpMulti = new LookUpMultiLanguage();
                    lookUpMulti = _LookUpMultiLanguageAccessor.Get(lookUp.Id, 1);

                    if (lookUpMulti == null)
                    {
                        response.Message = "please try again later";
                        response.ServerMessage = "lookup multi is null for lookup id: " + lookUp.Id;
                        response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;

                        return response;
                    }

                    data.Add(new ReturnedObject
                    {
                        Id = item.Id,
                        Name = lookUpMulti.Description
                    });

                }

                response.Data = data;
                response.HttpStatusCode = System.Net.HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ServerMessage = "countries function. " + ex.Message;
                response.Message = "Please try again later.";

                return response;
            }
        }

        public DynamicResponse<List<ReturnedObject>> GetCities(int countryId)
        {

            DynamicResponse<List<ReturnedObject>> response = new DynamicResponse<List<ReturnedObject>>();
            try
            {
                List<ReturnedObject> data = new List<ReturnedObject>();

                CityAccessor _CityAccessor = new CityAccessor();
                List<City> cities = _CityAccessor.GetList(countryId);

                LookUp lookUp = new LookUp();
                LookUpMultiLanguage lookUpMulti = new LookUpMultiLanguage();

                LookUpAccessor _LookUpAccessor = new LookUpAccessor();
                LookUpMultiLanguageAccessor _LookUpMultiLanguageAccessor = new LookUpMultiLanguageAccessor();

                foreach (City item in cities)
                {
                    lookUp = new LookUp();
                    lookUp = _LookUpAccessor.Get(item.Code, cityTableId);

                    if (lookUp == null)
                    {
                        response.Message = "please try again later";
                        response.ServerMessage = "lookup is null for city code: " + item.Code;
                        response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;

                        return response;
                    }

                    lookUpMulti = new LookUpMultiLanguage();
                    lookUpMulti = _LookUpMultiLanguageAccessor.Get(lookUp.Id, 1);

                    if (lookUpMulti == null)
                    {
                        response.Message = "please try again later";
                        response.ServerMessage = "lookup multi is null for lookup id: " + lookUp.Id;
                        response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;

                        return response;
                    }

                    data.Add(new ReturnedObject
                    {
                        Id = item.Id,
                        Name = lookUpMulti.Description
                    });

                }

                response.Data = data;
                response.HttpStatusCode = System.Net.HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                response.ServerMessage = "cities function. " + ex.Message;
                response.Message = "Please try again later.";

                return response;
            }
        }


        #endregion

        #endregion
    }

    public class ReturnedObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
