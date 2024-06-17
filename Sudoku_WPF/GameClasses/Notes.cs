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
    /// <summary>
    /// Represents a grid of notes for a Sudoku game.
    /// This class provides methods to manipulate the notes, including adding, removing, and clearing notes.
    /// </summary>
    public class Notes : UniformGrid
    {
        /// <summary>
        /// List of TextBlock elements representing the notes.
        /// </summary>
        public List<TextBlock> notes;

        /// <summary>
        /// Initializes a new instance of the Notes class and clears any existing notes.
        /// </summary>
        public Notes()
        {
            notes = new List<TextBlock>();
            Clear();
        }

        /// <summary>
        /// Adds or removes a note based on its existence.
        /// </summary>
        /// <param name="noteText">The text of the note to manipulate.</param>
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

        /// <summary>
        /// Adds a note with the specified text if notes are allowed.
        /// </summary>
        /// <param name="noteText">The text of the note to add.</param>
        public void AddNote(string noteText)
        {
            if (Settings.allowNotes)
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

        /// <summary>
        /// Creates a new TextBlock for a note with the specified text.
        /// </summary>
        /// <param name="noteText">The text of the note.</param>
        /// <returns>A TextBlock element with the specified note text.</returns>
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

        /// <summary>
        /// Clears all notes from the grid.
        /// </summary>
        public void Clear()
        {
            notes.Clear();
            Children.Clear();

            MyUpdateLayout();
        }

        /// <summary>
        /// Determines if there is only one note remaining.
        /// </summary>
        /// <returns>True if there is only one note, otherwise false.</returns>
        public bool IsLastOne()
        {
            return notes.Count == 1;
        }

        /// <summary>
        /// Gets the text of the last remaining note.
        /// </summary>
        /// <returns>The text of the last remaining note.</returns>
        public string LastOne()
        {
            return notes[0].Text;
        }

        /// <summary>
        /// Checks if a note with the specified text exists.
        /// </summary>
        /// <param name="noteText">The text of the note to check.</param>
        /// <returns>True if the note exists, otherwise false.</returns>
        public bool NoteExist(string noteText)
        {
            return notes.Any(note => note.Text == noteText);
        }

        /// <summary>
        /// Removes a note with the specified text.
        /// </summary>
        /// <param name="noteText">The text of the note to remove.</param>
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

        /// <summary>
        /// Removes a note at the specified index.
        /// </summary>
        /// <param name="idx">The index of the note to remove.</param>
        public void RemoveNoteByIdx(int idx)
        {
            if (idx < notes.Count && idx >= 0)
            {
                Children.RemoveAt(idx);
                notes.RemoveAt(idx);
                MyUpdateLayout();
            }
        }

        /// <summary>
        /// Updates the layout of the grid based on the number of notes.
        /// </summary>
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
