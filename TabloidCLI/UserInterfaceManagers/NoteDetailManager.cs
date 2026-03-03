using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    internal class NoteDetailManager : IUserInterfaceManager
    {
        private IUserInterfaceManager _parentUI;
        private PostRepository _postRepository;
        private NoteRepository _noteRepository;
        private int _postId;

        public NoteDetailManager(IUserInterfaceManager parentUI, string connectionString, int postId)
        {
            _parentUI = parentUI;
            _postRepository = new PostRepository(connectionString);
            _noteRepository = new NoteRepository(connectionString); 
            _postId = postId;
        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine(" 1) List Notes");
            Console.WriteLine(" 2) Add Note");
            Console.WriteLine(" 3) Remove Note");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ListNotes();
                    return this;
                case "2":
                    AddNote();
                    return this;
                case "3":
                    RemoveNote();
                    return this;
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void ListNotes()
        {
            List<Note> notes = _noteRepository.GetAll();
            foreach (Note note in notes)
            {
                Console.WriteLine("");
                Console.WriteLine(note.Title);
                Console.WriteLine("");
                Console.WriteLine(note.Content);
                Console.WriteLine("");
                Console.WriteLine(note.CreateDateTime);
                Console.WriteLine("");


            }
        }

        private void AddNote()
        {
            Console.WriteLine("New Note");
            Note note = new Note();

            Console.WriteLine("Title:");
            note.Title = Console.ReadLine();

            Console.WriteLine("Enter Note Here:");
            note.Content = Console.ReadLine();

            note.CreateDateTime = DateTime.Now;

            note.PostId = _postId;

            _noteRepository.Insert(note);
        }

        private Note Choose(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose a note:";
            }

            Console.WriteLine(prompt);

            List<Note> notes = _noteRepository.GetAll();

            for (int i = 0; i < notes.Count; i++)
            {
                Note note = notes[i];
                Console.WriteLine($" {i + 1}) {note.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return notes[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        private void RemoveNote()
        {
            Note noteToDelete = Choose("Which author would you like to remove?");
            if (noteToDelete != null)
            {
                _noteRepository.Delete(noteToDelete.Id);
            }
        }

        }
  }

