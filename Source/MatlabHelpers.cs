using SimioAPI;
using System;
using System.Collections.Generic;
using System.Text;


namespace MatlabStep
{
    /// <summary>
    /// Helper classes for Matlab
    /// </summary>
    public class MatlabHelpers
    {

        /// <summary>
        /// *** DO NOT USE ***
        /// This is a sample showing how not to call MLApp.
        /// This will invoke a new MLApp with each call.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="folderAddress">Where the function is located</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool CallMatlabFunctionInefficient(string functionName, String folderAddress, out string explanation)
        {
            explanation = "";
            try
            {
                // Create the MATLAB instance 
                MLApp.MLApp matlab = new MLApp.MLApp();

                string result = matlab.Execute("a = [1 2 3 4; 5 6 7 8]");

                // Call a MATLAB command to change the file location to where the file that holds the function is located.
                string cmd = $@"cd {folderAddress}";
                matlab.Execute(cmd);

                // Call the function
                matlab.Execute(functionName);

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"MATLAB call to Function={functionName} Folder={folderAddress} failed. Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Show calling the singleton to get a reference to the MLApp object.
        /// A new COM object will only be invoked if there is not one already.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="folderAddress">Where the function is located</param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool CallMatlabFunctionBetter(string functionName, String folderAddress, out string explanation)
        {
            explanation = "";
            try
            {
                // Call the Matlab context (singleton) to get the MLApp object.
                MLApp.MLApp matlab = MatlabContext.Instance.matlab;

                string result = matlab.Execute("a = [1 2 3 4; 5 6 7 8]");

                // Call a MATLAB command to change the file location to where the file that holds the function is located.
                string cmd = $@"cd {folderAddress}";
                matlab.Execute(cmd);

                // Call the function
                matlab.Execute(functionName);

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"MATLAB call to Function={functionName} Folder={folderAddress} failed. Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Logging displays a Trace line in Simio when the user has tracing turned on.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public static void LogIt(IStepExecutionContext context, string message)
        {
            context.ExecutionInformation.TraceInformation($"Matlab: {message}");
        }

        /// <summary>
        /// Alert produces a user dialog box.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public static void Alert(IStepExecutionContext context, string message)
        {
            context.ExecutionInformation.ReportError($"Matlab: Error={message}");
        }

    }
}
