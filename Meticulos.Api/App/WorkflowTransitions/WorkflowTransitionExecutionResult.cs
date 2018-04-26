using Meticulos.Api.App.Screens;
using System.Collections.Generic;

namespace Meticulos.Api.App.WorkflowTransitions
{
    public class WorkflowTransitionExecutionResult
    {
        public WorkflowTransitionExecutionResult()
        {
            ErrorMessages = new List<string>();
            Screens = new List<Screen>();
        }

        public List<string> ErrorMessages { get; set; }
        public List<Screen> Screens { get; set; }

        public bool IsSuccess
        {
            get { return ErrorMessages == null || ErrorMessages.Count == 0; }
        }
    }
}
