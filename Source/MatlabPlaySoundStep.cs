using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MatlabStep;
using SimioAPI;
using SimioAPI.Extensions;

namespace MatlabSteps
{
    /// <summary>
    /// Called by Simio when the model is loaded (design-time)
    /// Creates our definitions that can be set in Simio designer
    /// </summary>
    public class MatlabPlaySoundStepDefinition : IStepDefinition
    {
        #region IStepDefinition Members

        /// <summary>
        /// Property returning the full name for this type of step. The name should contain no spaces. 
        /// </summary>
        public string Name
        {
            get { return "MatlabPlaySoundStep"; }
        }

        /// <summary>
        /// Property returning a short description of what the step does.  
        /// </summary>
        public string Description
        {
            get { return "This step uses MATLAB to play a sound file. Simply enter the 'sound file path; and “folder address” and path to the matlab files. A file called PlaySoundFile.m is assumed. Tested for Matlab R2020a."; }        }

        /// <summary>
        /// Property returning an icon to display for the step in the UI. 
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return null; }
        }

        /// <summary>
        /// Property returning a unique static GUID for the step.  
        /// If creating your own step, use Tools > Create GUID to build your own
        /// </summary>
        public Guid UniqueID
        {
            get { return MY_ID; }
        }

        // Changed 29Jul2020/DHouck 
        static readonly Guid MY_ID = new Guid("{6A3C1C74-ECEF-49F2-9157-8778B86E4574}");

        /// <summary>
        /// Property returning the number of exits out of the step. Can return either 1 or 2. 
        /// </summary>
        public int NumberOfExits
        {
            get { return 2; }
        }

        /// <summary>
        /// Method called that defines the property schema for the step.
        /// </summary>
        public void DefineSchema(IPropertyDefinitions schema)
        {
            // Two IPropertyDefinition is needed to address "Function Name" and its "Folder Address"
            IPropertyDefinition pd1; //This is for MATLAB "Function Name"
            IPropertyDefinition pd2; //This is for MATLAB "Folder Address"

            // Sound File path
            pd1 = schema.AddStringProperty("SoundFilePath", string.Empty);
            pd1.DisplayName = "Sound File Path";
            pd1.Description = @"Full path to sound file location (e.g. c:\test\soundFiles\ship.wav)";
            pd1.Required = true;

            // MATLAB folder path
            pd2 = schema.AddStringProperty("MatlabFolderPath", string.Empty);
            pd2.DisplayName = "MATLAB Folder Path";
            pd2.Description = "The folder where the MATLAB files (e.g. *.m) are located, like 'C:\\MatlabFiles'";
            pd2.Required = true;
        }

        /// <summary>
        /// Method called to create a new instance of this step type to place in a process. 
        /// Returns an instance of the class implementing the IStep interface.
        /// </summary>
        public IStep CreateStep(IPropertyReaders properties)
        {
            return new MatlabPlaySoundStep(properties);
        }

        #endregion
    }

    class MatlabPlaySoundStep : IStep
    {
        IPropertyReaders _properties;
        IPropertyReader _SoundFileProp;
        IPropertyReader _MatlabFolderProp;
        string _SoundFile = "";
        string _MatlabFolder = "";

        /// <summary>
        /// Constructor. Initialize the property readers
        /// </summary>
        /// <param name="properties"></param>
        public MatlabPlaySoundStep(IPropertyReaders properties)
        {
            _properties = properties;
            _SoundFileProp = _properties.GetProperty("SoundFilePath");
            _MatlabFolderProp = _properties.GetProperty("MatlabFolderPath");

            // Call the Matlab context (singleton) simply to get it loaded.
            MLApp.MLApp matlab = MatlabContext.Instance.matlab;
        }

        #region IStep Members

        /// <summary>
        /// Method called when a process token executes the step.
        /// Using the location and name of the MATLAB function, make the call
        /// If error, then log the error and take the alternate exit.
        /// </summary>
        public ExitType Execute(IStepExecutionContext context)
        {
            // Get the properties of this step.

            if ( string.IsNullOrEmpty(_SoundFile))
                _SoundFile = _SoundFileProp.GetStringValue(context);

            if ( string.IsNullOrEmpty(_MatlabFolder))
                _MatlabFolder = _MatlabFolderProp.GetStringValue(context);
            
            //Using CallMtlab.cs in order to call Matlab function from its located folder
            MatlabHelpers MyMatlab = new MatlabHelpers();

            //  Note that we build the ChangeDirectory (cd) command
            if ( !CallMatlabPlaySoundFile( _MatlabFolder, _SoundFile, out string explanation) )
            {
                context.ExecutionInformation.ReportError(explanation);
                return ExitType.AlternateExit;
            }
            else
                return ExitType.FirstExit;
        }
        #endregion

        #region MATLAB logic
        /// <summary>
        /// Called by the the custom Matlab Step
        /// </summary>
        /// <param name="matlabFolder">Where the MATLAB function files are/param>
        /// <param name="soundFilePath">Full path to the sound file</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool CallMatlabPlaySoundFile(String matlabFolder, string soundFilePath, out string explanation)
        {
            explanation = "";
            string marker = "Begin.";
            try
            {

                marker = $"Getting reference to MATLAB app";
                // Call the Matlab context (singleton) to get the MLApp object.
                MLApp.MLApp matlab = MatlabContext.Instance.matlab;


                marker = "Checking folder and file";
                // Check for folder and file
                if (!Directory.Exists(matlabFolder))
                {
                    explanation = $"Cannot find MATLAB file folder={matlabFolder}";
                    return false;
                }
                // Check for folder and file
                if (!File.Exists(soundFilePath))
                {
                    explanation = $"Cannot find File={soundFilePath}";
                    return false;
                }

                marker = "Setting directory location for MATLAB files";
                // Call a MATLAB command to change the file location to where the file that holds the function is located.
                string cmd = $@"cd {matlabFolder}";
                matlab.Execute(cmd);

                marker = "Calling the MATLAB function";
                // Build the MATLAB command
                string matlabCommand = $@"PlaySoundFile(""{soundFilePath}"")";

                marker = "Getting the result";
                // A successful answer (when trimmed) starts with "ans = " followed by whatever the function return.
                string result = matlab.Execute(matlabCommand);

                if ( !result.Trim().StartsWith("ans ="))
                {
                    explanation = $"MATLAB Failure. SoundFile={soundFilePath} Result={result.Trim()}";
                    return false;
                }
                else
                    return true;

            }
            catch (Exception ex)
            {
                explanation = $"MATLAB call. Marker={marker} to play Folder={matlabFolder} file={soundFilePath} failed. Err={ex}";
                return false;
            }
        }

        #endregion
    }
}
