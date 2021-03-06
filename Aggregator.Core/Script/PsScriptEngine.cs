﻿using System.Collections.Generic;
using System.Management.Automation.Runspaces;

using Aggregator.Core.Interfaces;
using Aggregator.Core.Monitoring;

namespace Aggregator.Core
{
    /// <summary>
    /// Invokes Powershell scripting engine
    /// </summary>
    public class PsScriptEngine : ScriptEngine
    {
        private readonly Dictionary<string, string> scripts = new Dictionary<string, string>();

        public PsScriptEngine(ILogEvents logger, bool debug)
            : base(logger, debug)
        {
        }

        public override bool Load(string scriptName, string script)
        {
            this.scripts.Add(scriptName, script);
            return true;
        }

        public override bool LoadCompleted()
        {
            return true;
        }

        public override void Run(string scriptName, IWorkItem workItem, IWorkItemRepository store)
        {
            string script = this.scripts[scriptName];

            var config = RunspaceConfiguration.Create();
            using (var runspace = RunspaceFactory.CreateRunspace(config))
            {
                runspace.Open();

                runspace.SessionStateProxy.SetVariable("self", workItem);
                runspace.SessionStateProxy.SetVariable("store", store);
                runspace.SessionStateProxy.SetVariable("logger", this.Logger.ScriptLogger);

                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(script);

                // execute
                var results = pipeline.Invoke();

                this.Logger.ResultsFromScriptRun(scriptName, results);
            }
        }
    }
}
