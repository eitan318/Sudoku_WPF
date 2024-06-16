using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Sudoku_WPF.publico;

namespace Sudoku_WPF.GameClasses
{
    public class Notes : UniformGrid
    {
        public List<TextBlock> notes;

        public Notes()
        {
            notes = new List<TextBlock>();
            Clear();
        }

        public void ManipulateNote(string noteText)
        {
            if (NoteExist(noteText))
            {
                RemoveNote(noteText);
            }
            else
            {
                AddNote(noteText);
            }
        }

        public void AddNote(string noteText)
        {
            if(Settings.allowNotes)
            {
                if (!string.IsNullOrEmpty(noteText))
                {
                    Viewbox noteContainer = new Viewbox();
                    noteContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
                    noteContainer.VerticalAlignment = VerticalAlignment.Stretch;

                    TextBlock note = NewNote(noteText);

                    // Add the TextBlock to the ViewBox
                    noteContainer.Child = note;

                    notes.Add(note);
                    Children.Add(noteContainer);

                    MyUpdateLayout();
                }
            }
            
        }

        private TextBlock NewNote(string noteText)
        {
            TextBlock note = new TextBlock()
            {
                Text = noteText,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                TextAlignment = TextAlignment.Center,
                IsHitTestVisible = false
            };

            // Set the Foreground property using resource reference
            note.SetResourceReference(TextBlock.ForegroundProperty, "Text");

            return note;
        }



        public void Clear()
        {
            notes.Clear();
            Children.Clear();

            MyUpdateLayout();
        }

        public bool IsLastOne()
        {
            return notes.Count == 1;
        }

        public string LastOne()
        {
            return notes[0].Text;
        }

        public bool NoteExist(string noteText)
        {
            return notes.Any(note => note.Text == noteText);
        }

        private void RemoveNote(string noteText)
        {
            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Text == noteText)
                {
                    RemoveNoteByIdx(i);
                    break;
                }
            }
        }

        public void RemoveNoteByIdx(int idx)
        {
            if (idx < notes.Count && idx >= 0)
            {
                Children.RemoveAt(idx);
                notes.RemoveAt(idx);
                MyUpdateLayout();
            }
        }

        private void MyUpdateLayout()
        {
            int noteCount = notes.Count;
            if (noteCount < 4)
            {
                UpdateLayout();
                return;
            }

            Columns = (int)Math.Ceiling(Math.Sqrt(noteCount));
            Rows = (int)Math.Ceiling(noteCount / (double)Columns);

            if (Columns * (Rows - 1) >= noteCount)
            {
                Rows--;
            }
        }
    }
}
