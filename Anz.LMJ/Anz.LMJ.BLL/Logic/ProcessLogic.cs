using Anz.LMJ.BLO.LogicObjects.CommonObjects;
using Anz.LMJ.BLO.LogicObjects.Submission;
using Anz.LMJ.DAL.Accessors;
using Anz.LMJ.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Anz.LMJ.BLL.Logic.Enums;

namespace Anz.LMJ.BLL.Logic
{
    public class ProcessLogic
    {
        private DynamicResponse<ProcessLO> GetProcess(long processId,long submissionId)
        {
            DynamicResponse<ProcessLO> response = new DynamicResponse<ProcessLO>();
            #region Accessors
            ProcessAccessor _ProcessAccessor = new ProcessAccessor();
            SubmissionFilesAccessor _SubmissionFilesAccessor = new SubmissionFilesAccessor();
            #endregion
            try
            {
                Process modelProcess = _ProcessAccessor.Get(processId);

                if(modelProcess == null)
                {
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later";
                    response.ServerMessage = "process empty";

                    return response;
                }
                else
                {
                    ProcessLO data = new ProcessLO();

                    data.Id = modelProcess.Id;
                    data.StageName = modelProcess.StageName;
                    data.ButtonValue = modelProcess.ButtonValue;
                    data.ButtonBackground = modelProcess.ButtonBackground;
                    data.isPreCopyediting = modelProcess.isPreCopyediting;
                    data.isPreProduction = modelProcess.isPreProduction;
                    data.isPreReview = modelProcess.isPreReview;

                    data.isModalRequired = modelProcess.isModalRequired;
                    data.ProcessIdinModal = modelProcess.ProcessIdinModal;
                    data.ModalName = modelProcess.ModalName;
                    data.ModalAction = modelProcess.ModelAction;

                    //check if is model and get the files
                    if(data.isModalRequired)
                    {
                        #region Logic
                        FileTypeLogic _FileTypeLogic = new FileTypeLogic();
                        #endregion

                        //get files of this model
                        List<SubmissionFile> files = new List<SubmissionFile>();
                        if(modelProcess.isPreReview)
                        {
                            files = _SubmissionFilesAccessor.GetListNullable(submissionId, true, null, null);
                        }
                        if (modelProcess.isPreCopyediting)
                        {
                            files = _SubmissionFilesAccessor.GetListNullable(submissionId, null, true, null);

                        }
                        if (modelProcess.isPreProduction)
                        {
                            files = _SubmissionFilesAccessor.GetListNullable(submissionId, null, null, true);

                        }

                        List<SubmissionFilesLO> processFiles = new List<SubmissionFilesLO>();
                        foreach (SubmissionFile item in files)
                        {
                            processFiles.Add(new SubmissionFilesLO {
                                Id = item.Id,
                                Name = item.FileName,
                                TypeName = _FileTypeLogic.GetFileType((long)item.ComponentId),
                                TypeId = (long)item.ComponentId
                            });
                        }

                        data.ModalFiles = processFiles;
                    }

                    response.HttpStatusCode = HttpStatusCode.OK;
                    response.Data = data;


                    return response;
                }

            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later";
                response.ServerMessage = ex.Message;

                return response;
            }
        }

        public DynamicResponse<List<ProcessLO>> GetNextProcessForSubmission(long submissionId, long roleId)
        {
            #region MyRegion
            ProcessStagesAccessor _ProcessStagesAccessor = new ProcessStagesAccessor();
            #endregion
            DynamicResponse<List<ProcessLO>> response = new DynamicResponse<List<ProcessLO>>();
            try
            {
                DynamicResponse<Process> lastProcess = GetLastProcessForSubmission(submissionId);

                //check if there is an error
                if (lastProcess.HttpStatusCode != HttpStatusCode.OK)
                {
                    response.HttpStatusCode = lastProcess.HttpStatusCode;
                    response.Message = lastProcess.Message;
                    response.ServerMessage = lastProcess.ServerMessage;

                    return response;
                }
                else
                {
                    List<ProcessLO> data = new List<ProcessLO>();

                    //get the next stages
                    List<ProcessStage> stages = new List<ProcessStage>();
                    stages = _ProcessStagesAccessor.GetList(lastProcess.Data.Id, roleId);

                  
                    //foreach stage get info
                    DynamicResponse<ProcessLO> processObject = new DynamicResponse<ProcessLO>();
                    foreach (ProcessStage item in stages)
                    {
                        //get process
                        processObject = new DynamicResponse<ProcessLO>();
                        processObject = GetProcess(item.NextProcessId,submissionId);

                        //check process
                        if(processObject.HttpStatusCode != HttpStatusCode.OK)
                        {
                            response.HttpStatusCode = lastProcess.HttpStatusCode;
                            response.Message = lastProcess.Message;
                            response.ServerMessage = lastProcess.ServerMessage;

                            return response;
                        }
                        else
                        {
                            data.Add(new ProcessLO
                            {
                                Id = (long)item.NextProcessId,
                                isIncludeSkip = (bool)item.includesSkip,
                                ModalAction = processObject.Data.ModalAction,
                                ButtonBackground = processObject.Data.ButtonBackground,
                                ButtonValue = processObject.Data.ButtonValue,
                                StageName = processObject.Data.StageName,
                                isModalRequired = processObject.Data.isModalRequired,
                                isPreCopyediting = processObject.Data.isPreCopyediting,
                                isPreProduction = processObject.Data.isPreProduction,
                                isPreReview = processObject.Data.isPreReview,
                                ModalName = processObject.Data.ModalName,
                                Name = processObject.Data.Name,
                                ProcessIdinModal = processObject.Data.ProcessIdinModal,
                                ModalFiles = processObject.Data.ModalFiles
                            });

                         
                        }                        
                    }

                    response.HttpStatusCode = HttpStatusCode.OK;
                    response.Data = data;

                    return response;

                }
                
            }
            catch (Exception ex)
            {
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later";
                response.Message = ex.Message;

                return response;
            }
        }

        public DynamicResponse<Process> GetLastProcessForSubmission(long submissionId)
        {
            #region Accessors
            SubmissionInProcessAccessor _SubmissionInProcessAccessor = new SubmissionInProcessAccessor();
            ProcessAccessor _ProcessAccessor = new ProcessAccessor();
            #endregion
            DynamicResponse<Process> response = new DynamicResponse<Process>();
            try
            {
                //get the last submission in process
                SubmissionInProcess submissionInProcess = _SubmissionInProcessAccessor.GetLast(submissionId);

                Process process = new Process();

                //if null, not assigned
                if (submissionInProcess == null)
                {
                    //get process of not assigned
                    process = _ProcessAccessor.Get(ProcessCodes.unassigned.ToString());

                }
                //else get the process
                else
                {
                    //get process by the id
                    process = _ProcessAccessor.Get((long)submissionInProcess.ProcessId);
                }

                if (process == null)
                {
                    //return error
                    response.HttpStatusCode = HttpStatusCode.InternalServerError;
                    response.Message = "Please try again later.";
                    response.ServerMessage = "Process in empty";

                    return response;
                }
                else
                {
                    //return response with displayed name of the process
                    response.HttpStatusCode = HttpStatusCode.OK;
                    response.Data = process;

                    return response;
                }
            }
            catch (Exception ex)
            {
                //return error
                response.HttpStatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Please try again later.";
                response.ServerMessage = ex.Message;

                return response;
            }
        }



    }
}
