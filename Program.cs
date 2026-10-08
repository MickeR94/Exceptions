namespace ExceptionsDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                Console.WriteLine("=== Start av programmet ===");

                // Exempel 1: try-catch-finally
                try
                {
                    Console.WriteLine("Försöker läsa fil och räkna...");
                    var path = Path.Combine(AppContext.BaseDirectory, "numbers2.txt");
                    var result = ProcessFile(path);
                  
                    Console.WriteLine($"\nResultat: {result}");
                }
                catch (FileNotFoundException ex) // There needed to be a corresponding one down below as well for this to work
                {
                    // Specifikt fel om filen inte finns
                    Console.WriteLine($"Filen hittades inte: {ex.Message}");
                }
                catch (FormatException ex)
                {
                    // Specifikt fel om texten inte kan tolkas som tal
                    Console.WriteLine($"Formatfel: {ex.Message}");
                }
                catch (DivideByZeroException ex)
                {
                    // Specifikt fel om nolldivision
                    Console.WriteLine($"Kan inte dividera med noll: {ex.Message}");
                }
                // Added a catch-block in case the directory can't be found
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine($"Can not find the directory: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Fallback för alla övriga obekanta fel
                    Console.WriteLine($"Okänt fel ***: {ex.Message}");
                    
                }
                finally
                {
                    // Körs ALLTID, även om det blev undantag
                    Console.WriteLine("Cleanup: Logging avslutat anrop.");
                }

                Console.WriteLine("Programmet avslutas normalt.");
            }

            // Exempel på metod som själv kastar ett undantag (throw)
            static double ProcessFile(string fileName)
            {
                // Om filnamnet är tomt: logiskt fel vi vill signalera
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException("Filnamn får inte vara tomt eller null.", nameof(fileName));
                }

                StreamReader? reader = null;
                try
                {
                    reader = new StreamReader(fileName);

                    // This one doesn't seem to do anything.
                    string? line = reader.ReadLine();
                    if (line == null)
                        throw new InvalidOperationException("Filen är tom.");

                    // Försöker omvandla text till tal
                    int number = int.Parse(line); // Kan ge FormatException

                    // Division: kan ge DivideByZeroException
                    // PS! Only works if there's an int. Originally it was a float. No DivideByZeroException according to the documentation.
                    // number contains whatever number that's read from numbers.txt
                    try
                    {
                        return 100 / number;
                    }
                    catch (DivideByZeroException ex)
                    {
                        Console.WriteLine($"Error --> Something went wrong with the calculation: {ex.Message}");
                        return -1;
                    }
                }
                catch (FormatException ex)
                {
                    // Vi kan logga eller omformulera felet
                    Console.WriteLine($"Formatfel i ProcessFile: {ex.Message}");
                    // Vi kan välja att låta metoden "kasta upp" felet
                    throw; // När du i `catch` bara vill logga/analysera,
                           // men låta anroparen (t.ex. en högre nivå i applikationen)
                           // bestämma hur man ska återhämta sig. 
                }
                catch (FileNotFoundException ex)
                {
                    throw new FileNotFoundException($"{ex.Message}");
                }
                catch (Exception ex)
                {
                    // Om vi vill ge en mer meningsfull feltyp till anroparen
                    throw new InvalidOperationException(
                    "Det gick inte att processa filen +++.",
                    ex); // InnerException = ursprunglig fel
                }
                finally
                {
                    // Garanterad stängning av resurs
                    reader?.Close();
                    Console.WriteLine("finally i ProcessFile: StreamReader stängd.");
                }
            }
        }
    }
}

