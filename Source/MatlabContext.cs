using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatlabStep
{
    /// <summary>
    /// A singleton to hold matlab execution context.
    /// This will prevent a 'load' of matlab each time we call.
    /// </summary>
    public sealed class MatlabContext
    {
        private static readonly MatlabContext _instance = new MatlabContext();

        public MLApp.MLApp matlab = null;

        /// <summary>
        /// Constructor
        /// </summary>
        static MatlabContext()
        {
        }

        /// <summary>
        /// private constructor.
        /// Create our matlab object.
        /// </summary>
        private MatlabContext() 
        {
            matlab = new MLApp.MLApp();
        }

        public static MatlabContext Instance { get { return _instance; } }


    }
}
