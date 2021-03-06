﻿using ERService.Infrastructure.Constants;
using ERService.TemplateEditor.Interpreter;
using ERService.TemplateEditor.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ERService.TemplateEditor
{
    public class TemplateEditorModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IInterpreter, Interpreter.Interpreter>();

            containerRegistry.RegisterForNavigation<PrintTemplateEditorView>(ViewNames.PrintTemplateEditorView);
        }
    }
}