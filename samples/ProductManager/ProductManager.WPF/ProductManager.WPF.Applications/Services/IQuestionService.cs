using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManager.WPF.Applications.Services
{
    public interface IQuestionService
    {
        bool? ShowQuestion(string message);

        bool ShowYesNoQuestion(string message);
    }
}
