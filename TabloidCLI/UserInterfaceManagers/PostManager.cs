using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{
    public class PostManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private PostRepository _postRepository;
        private AuthorRepository _authorRepository;
        private BlogRepository _blogRepository;
        private string _connectionString;

        public PostManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _postRepository = new PostRepository(connectionString);
            _authorRepository = new AuthorRepository(connectionString);
            _blogRepository = new BlogRepository(connectionString);
            _connectionString = connectionString;
        }

        //creates the menu for post functions
        public IUserInterfaceManager Execute()
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("Post Menu");
            Console.WriteLine(" 1) List Posts");
            Console.WriteLine(" 2) Add Post");
            Console.WriteLine(" 3) Edit Post");
            Console.WriteLine(" 4) Remove Post");
            Console.WriteLine(" 5) View Post Details");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    List();
                    return this;
                case "2":
                    Add();
                    return this;
                case "3":
                    Edit();
                    return this;
                case "4":
                    Remove();
                    return this;
                case "5":
                    Post post = Choose();
                    if (post == null)
                    {
                        return this;
                    }
                    else
                    {
                        return new PostDetailManager(this, _connectionString, post.Id);
                    }

                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        //creates list of all journal entries
        //not sure what I'm doing wrong
        private void List()
        {
            List<Post> posts = _postRepository.GetAll();
            foreach (Post post in posts)
            {
                Console.WriteLine($"{post.Title}: {post.Url}");
            }
        }

        private Post Choose(string prompt = null)
        {
            if (prompt == null)
            {
                prompt = "Please choose a Post:";
            }

            Console.WriteLine(prompt);

            List<Post> posts = _postRepository.GetAll();

            for (int i = 0; i < posts.Count; i++)
            {
                Post post = posts[i];
                Console.WriteLine($" {i + 1}) {post.Title}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return posts[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

        //allows user to add new post
        private void Add()
        {
            Console.WriteLine("New Post");
            Post post = new Post();

            Console.Write("Title: ");
            post.Title = Console.ReadLine();

            Console.Write("Url: ");
            post.Url = Console.ReadLine();

            //Console.Write("CreateDateTime: ");
            //ask group what kind of date we're using here
            //like Convert.ToDateTime(Console.ReadLine()) ?
            //this works and posts to database, but obv the date is datetime.now
            post.PublishDateTime = DateTime.Now;


            // list authors
            //foreach author in authors
            //console write line a.id a.fullname
            //console write line "> "
            //int choice = int.Parse(readline)
            //author author = authors[choice - 1]
            Console.WriteLine("Please choose an author:");
            List<Author> authors = _authorRepository.GetAll();
            foreach (Author singleAuthor in authors)
            {
                Console.WriteLine($"{singleAuthor.Id} {singleAuthor.FullName}");
                Console.WriteLine();
            }

            Console.WriteLine("> ");
            int choice = int.Parse(Console.ReadLine());
            Author author = authors[choice - 1];

            post.Author = author;

            Console.WriteLine("Please choose a blog:");
            List<Blog> blogs = _blogRepository.GetAll();

            foreach (Blog singleBlog in blogs)
            {
                Console.WriteLine($"{singleBlog.Id} {singleBlog.Title}");
            }
            Console.WriteLine("> ");
            int blogChoice = int.Parse(Console.ReadLine());
            Blog blog = blogs[choice - 1];

            post.Blog = blog;

            _postRepository.Insert(post);

        }

        //This allows user to edit a journal entry but not change the Date Created
        private void Edit()
        {
            Post postToEdit = Choose("Which post would you like to edit?");
            if (postToEdit == null)
            {
                return;
            }

            Console.WriteLine();
            Console.Write("New Title (blank to leave unchanged: ");
            string title = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(title))
            {
                postToEdit.Title = title;
            }
            Console.Write("New Url (blank to leave unchanged: ");
            string content = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(content))
            {
                postToEdit.Url = content;
            }

            _postRepository.Update(postToEdit);
        }

        //This allows user to delete a journal entry
        private void Remove()
        {
            Post postToDelete = Choose("Which post would you like to remove?");
            if (postToDelete != null)
            {
                _postRepository.Delete(postToDelete.Id);
            }
        }
    }
}
