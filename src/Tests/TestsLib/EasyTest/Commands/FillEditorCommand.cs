﻿using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.TestsLib.EasyTest.Commands{
    public class FillEditorCommand:EasyTestCommand{
        private readonly bool _inlineEditor;
        public const string Name = "FillEditor";

        public FillEditorCommand(string editor,string value,bool inlineEditor=false){
            _inlineEditor = inlineEditor;
            if (inlineEditor){
                Parameters.Add(new Parameter($"Columns = {editor}"));
                Parameters.Add(new Parameter($"Values = {value}"));
            }
            else{
                Parameters.Add(new Parameter(editor,value));
            }
        }

        protected override void ExecuteCore(ICommandAdapter adapter){
            var command = _inlineEditor?(Command) this.ConnvertTo<FillRecordCommand>():this.ConnvertTo<FillFieldCommand>();
            adapter.Execute(command);
        }
    }
}