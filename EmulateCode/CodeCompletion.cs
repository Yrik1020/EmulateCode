using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace EmulateCode
{
    public class CodeCompletion
    {
        private readonly TextEditor _editor;
        private readonly CompletionWindow _completionWindow;

        public CodeCompletion(TextEditor editor)
        {
            _editor = editor;
            _completionWindow = new CompletionWindow(editor.TextArea);
            _completionWindow.Closed += OnCompletionWindowClosed;
        }

        public void HandleTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Char.IsLetterOrDigit(e.Text[0]) || e.Text[0] == '_')
            {
                var offset = _editor.CaretOffset;
                var word = GetWordUnderCaret(offset);
                var completions = GetCompletionsForWord(word);

                if (completions.Count > 0)
                {
                    _completionWindow.StartOffset = offset - word.Length;
                    foreach (var completion in completions)
                    {
                        _completionWindow.CompletionList.CompletionData.Add(completion);
                    }
                    _completionWindow.Show();
                    _completionWindow.Closed += OnCompletionWindowClosed;
                }
            }
        }


        private string GetWordUnderCaret(int offset)
        {
            var document = _editor.Document;
            var line = document.GetLineByOffset(offset);
            var startOffset = line.Offset;
            var endOffset = offset;

            while (endOffset < line.EndOffset && Char.IsLetterOrDigit(document.GetCharAt(endOffset)))
            {
                endOffset++;
            }

            while (startOffset > line.Offset && Char.IsLetterOrDigit(document.GetCharAt(startOffset - 1)))
            {
                startOffset--;
            }

            return document.GetText(startOffset, endOffset - startOffset);
        }

        private List<ICompletionData> GetCompletionsForWord(string word)
        {
            // Здесь вы можете добавить свои собственные подсказки на основе текущего слова
            // Например:
            // if (word == "Console")
            // {
            //     return new List<ICompletionData> { new MyCompletionData("WriteLine") };
            // }
            return new List<ICompletionData>();
        }

        private void OnCompletionWindowClosed(object sender, EventArgs e)
        {
            _completionWindow.Closed -= OnCompletionWindowClosed;
            _completionWindow.CompletionList.CompletionData.Clear();
        }
    }
}
