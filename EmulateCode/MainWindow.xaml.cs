using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CSharp;

namespace EmulateCode
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Добавьте начальный код сюда
            codeEditor.Text = "using System;\nusing System.Collections.Generic;\n\npublic class Program\n{\n    public static void Main()\n    {\n        // Ваш код здесь\n    }\n}";

            codeEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            codeEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
        }


        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && char.IsLetterOrDigit(e.Text[0]))
            {
                ShowCompletion(codeEditor.TextArea);
            }
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                ShowCompletion(codeEditor.TextArea);
            }
        }

        private void ShowCompletion(TextArea textArea)
        {
            CompletionWindow completionWindow = new CompletionWindow(textArea);
            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

            // Добавьте свои подсказки здесь
            data.Add(new CompletionData("async"));
            data.Add(new CompletionData("Console.WriteLine"));
            data.Add(new CompletionData("await"));
            data.Add(new CompletionData("using"));
            data.Add(new CompletionData("namespace"));
            data.Add(new CompletionData("class"));
            data.Add(new CompletionData("struct"));
            data.Add(new CompletionData("interface"));
            data.Add(new CompletionData("enum"));
            data.Add(new CompletionData("delegate"));
            data.Add(new CompletionData("event"));
            data.Add(new CompletionData("void"));
            data.Add(new CompletionData("int"));
            data.Add(new CompletionData("float"));
            data.Add(new CompletionData("double"));
            data.Add(new CompletionData("string"));
            data.Add(new CompletionData("char"));
            data.Add(new CompletionData("bool"));
            data.Add(new CompletionData("var"));
            data.Add(new CompletionData("List<>"));
            data.Add(new CompletionData("Dictionary<,>"));
            data.Add(new CompletionData("IEnumerable<>"));
            data.Add(new CompletionData("LINQ"));
            data.Add(new CompletionData("Lambda"));
            data.Add(new CompletionData("try"));
            data.Add(new CompletionData("catch"));
            data.Add(new CompletionData("finally"));
            data.Add(new CompletionData("throw"));
            data.Add(new CompletionData("if"));
            data.Add(new CompletionData("else"));
            data.Add(new CompletionData("switch"));
            data.Add(new CompletionData("case"));
            data.Add(new CompletionData("default"));
            data.Add(new CompletionData("break"));
            data.Add(new CompletionData("return"));
            data.Add(new CompletionData("for"));
            data.Add(new CompletionData("foreach"));
            data.Add(new CompletionData("while"));
            data.Add(new CompletionData("do"));
            data.Add(new CompletionData("goto"));
            data.Add(new CompletionData("new"));
            data.Add(new CompletionData("this"));
            data.Add(new CompletionData("base"));
            data.Add(new CompletionData("public"));
            data.Add(new CompletionData("private"));
            data.Add(new CompletionData("protected"));
            data.Add(new CompletionData("internal"));
            data.Add(new CompletionData("static"));
            data.Add(new CompletionData("readonly"));
            data.Add(new CompletionData("const"));
            data.Add(new CompletionData("virtual"));
            data.Add(new CompletionData("override"));
            data.Add(new CompletionData("abstract"));
            data.Add(new CompletionData("sealed"));
            data.Add(new CompletionData("partial"));
            data.Add(new CompletionData("class"));
            data.Add(new CompletionData("struct"));
            data.Add(new CompletionData("interface"));
            data.Add(new CompletionData("enum"));
            data.Add(new CompletionData("delegate"));
            data.Add(new CompletionData("event"));
            data.Add(new CompletionData("void"));
            data.Add(new CompletionData("int"));
            data.Add(new CompletionData("float"));
            data.Add(new CompletionData("string"));
            data.Add(new CompletionData("bool"));
            data.Add(new CompletionData("var"));
            data.Add(new CompletionData("List<>"));
            data.Add(new CompletionData("Dictionary<,>"));
            data.Add(new CompletionData("IEnumerable<>"));



            completionWindow.Show();
            completionWindow.Closed += delegate { completionWindow = null; };
        }

        private void RunCode_Click(object sender, RoutedEventArgs e)
        {
            debugOutput.Text = null;
            string code = codeEditor.Text;

            string fileName = System.IO.Path.GetTempFileName();
            string outputFileName = System.IO.Path.ChangeExtension(fileName, "exe");

            System.IO.File.WriteAllText(fileName, code);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outputFileName;

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    debugOutput.Text += $"Ошибка компиляции: {error.ErrorText}\n";
                }
            }
            else
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = outputFileName;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                debugOutput.Text += output;
            }

            // Очистка временных файлов
            System.IO.File.Delete(fileName);
            System.IO.File.Delete(outputFileName);
        }

        private void debugOutput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            debugOutput.ScrollToEnd();
        }

        private void codeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            TextEditor editor = (TextEditor)sender;

            if (e.Key == Key.OemOpenBrackets && Keyboard.Modifiers == ModifierKeys.None) // Если нажата фигурная открывающая скобка ({) без модификаторов
            {
                int caretOffset = editor.CaretOffset;
                editor.Document.Insert(caretOffset, "{}");
                editor.CaretOffset = caretOffset + 1; // Перемещаем курсор между скобками
                editor.Document.Insert(caretOffset + 1, "\n    \n}"); // Вставляем новую строку с отступом
                editor.CaretOffset = caretOffset + 4; // Устанавливаем курсор на новую строку с отступом
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
        }

        private void codeEditor_KeyDown(object sender, TextCompositionEventArgs e)
        {
            TextEditor editor = (TextEditor)sender;

            if (e.Text == "{" && editor.CaretOffset < editor.Text.Length && editor.Text[editor.CaretOffset] == '}')
            {
                // Если следующий символ после курсора - закрывающая скобка, просто передвигаем курсор
                editor.CaretOffset++;
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
            else if (e.Text == "{")
            {
                int caretOffset = editor.CaretOffset;
                editor.Document.Insert(caretOffset, "{}");
                editor.CaretOffset = caretOffset + 1; // Перемещаем курсор между скобками
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
            else if (e.Text == "}" && editor.CaretOffset < editor.Text.Length && editor.Text[editor.CaretOffset] == '}')
            {
                // Если следующий символ после курсора - закрывающая скобка, просто передвигаем курсор
                editor.CaretOffset++;
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
            else if (e.Text == "(" && editor.CaretOffset < editor.Text.Length && editor.Text[editor.CaretOffset] == ')')
            {
                // Если следующий символ после курсора - закрывающая скобка, просто передвигаем курсор
                editor.CaretOffset++;
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
            else if (e.Text == "(")
            {
                int caretOffset = editor.CaretOffset;
                editor.Document.Insert(caretOffset, "()");
                editor.CaretOffset = caretOffset + 1;
                e.Handled = true;
            }
            else if (e.Text == ")" && editor.CaretOffset < editor.Text.Length && editor.Text[editor.CaretOffset] == ')')
            {
                // Если следующий символ после курсора - закрывающая скобка, просто передвигаем курсор
                editor.CaretOffset++;
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
            else if (e.Text == "[" && editor.CaretOffset < editor.Text.Length && editor.Text[editor.CaretOffset] == ']')
            {
                // Если следующий символ после курсора - закрывающая скобка, просто передвигаем курсор
                editor.CaretOffset++;
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
            else if (e.Text == "[")
            {
                int caretOffset = editor.CaretOffset;
                editor.Document.Insert(caretOffset, "[]");
                editor.CaretOffset = caretOffset + 1;
                e.Handled = true;
            }
            else if (e.Text == "]" && editor.CaretOffset < editor.Text.Length && editor.Text[editor.CaretOffset] == ']')
            {
                // Если следующий символ после курсора - закрывающая скобка, просто передвигаем курсор
                editor.CaretOffset++;
                e.Handled = true; // Предотвращаем дальнейшую обработку события
            }
        }
    }
    public class CompletionData : ICompletionData
    {
        public CompletionData(string text)
        {
            this.Text = text;
        }
        public System.Windows.Media.ImageSource Image => null;

        public string Text { get; }

        public object Content => this.Text;

        public object Description => "Описание подсказки";

        public double Priority => 0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            var document = textArea.Document;
            var currentText = document.GetText(completionSegment);

            // Find the start of the word (if any)
            int startOffset = completionSegment.Offset - 1;
            while (startOffset >= 0 && char.IsLetterOrDigit(document.GetCharAt(startOffset)))
            {
                startOffset--;
            }
            startOffset++;

            // Calculate the length to replace
            int length = completionSegment.EndOffset - startOffset;

            document.Replace(startOffset, length, this.Text);
        }
    }




}
