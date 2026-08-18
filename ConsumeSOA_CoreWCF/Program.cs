
//using BookLibrary_WCFService; //wsdl
using LibraryServiceReference;
using System.ServiceModel;

class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Connecting to CoreWCF Service...");

        // 1. Define the binding matching your server configuration
        var binding = new BasicHttpBinding();

        // 2. Define the exact endpoint address of your running API
        var endpoint = new EndpointAddress("http://localhost:5150/Services/LibraryService.svc?wsdl");

        // 3. Initialize the generated client proxy
        // (Note: Name might vary slightly depending on your tool, e.g., LibraryServiceClient)
        var client = new LibraryServiceClient(binding, endpoint);

        try
        {
            Console.WriteLine("Adding book...");

            BookDataContract? newBook = new()
            {
                Title = "First Book",
                IsAvailable = true
            };

            var result = await client.AddBookAsync(newBook);

            Console.WriteLine("Fetching book details...");

            // 4. Invoke the async endpoint smoothly
            var book = await client.GetBookByIdAsync(1);

            Console.WriteLine($"\nSuccess! Book Found:");
            Console.WriteLine($"ID: {book.Id}");
            Console.WriteLine($"Title: {book.Title}");
            Console.WriteLine($"Available: {book.IsAvailable}");

            // Add
            var addBook = new BookDataContract { IsAvailable = false, Title = "New Book" };
            var addResult = await client.AddBookAsync(addBook);
            Console.WriteLine($"Book added, new id: {addResult.NewId}");

            // Update
            var id = addResult.NewId;
            var updateBook = new BookDataContract
            {
                Id = id,
                IsAvailable = true,
                Title = "Updated Book"
            };

            // Update
            var updateResult = await client.UpdateBookAsync(updateBook);
            if (updateResult)
            {
                var findBook = await client.GetBookByIdAsync(id);
                Console.WriteLine($"Updated book, title: {findBook.Title}");
            }

            // Delete
            Console.WriteLine("Deleting book");
            var deleteResult = await client.RemoveBookAsync(id);
            var getBook = await client.GetBookByIdAsync(id);
        }
        // 5. Catch your custom strongly-typed CoreWCF faults
        catch (FaultException<BookFault> ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n[Server Business Error]: {ex.Detail.ErrorMessage}");
            Console.WriteLine($"[Fault Reason]: {ex.Message}");
        }
        // 6. Catch general communication or connection drops
        catch (CommunicationException ex)
        {
            Console.WriteLine($"\n[Network/Protocol Error]: Check if server is running! {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[General Error]: {ex.Message}");
        }
        finally
        {
            // 7. Always cleanly close the client channel pipeline
            await client.CloseAsync();
        }

        Console.ReadLine();
    }
    
    
}


