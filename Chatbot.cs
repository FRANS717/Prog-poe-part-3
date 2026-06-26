using System;
using System.Collections.Generic;
using System.Linq;

namespace PROG6221_V1
{
    /// <summary>
    /// Main chatbot class handling user interaction and responses.
    /// </summary>
    public class Chatbot
    {
        private readonly string userName;
        private bool isRunning;

        // Dictionaries to store questions and answers for each topic
        private readonly Dictionary<string, string> generalResponses;
        private readonly Dictionary<string, string> phishingResponses;
        private readonly Dictionary<string, string> passwordResponses;
        private readonly Dictionary<string, string> safeBrowsingResponses;

        /// <summary>
        /// Initializes a new instance of the Chatbot class with the user's name.
        /// </summary>
        public Chatbot(string userName)
        {
            this.userName = userName;
            isRunning = true;

            // Initialize response dictionaries
            generalResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how are you", $"I'm just a program, {userName}, but I'm here and ready to help you stay safe online! 😊" },
                { "what is your purpose", $"My purpose is to educate you about cybersecurity, {userName}. I can answer questions about phishing, password safety, and safe browsing." },
                { "what can i ask you about", $"You can ask me about:\n• Phishing emails and scams\n• Password safety and best practices\n• Safe browsing habits\n• General cybersecurity awareness" },
                { "who created you", "I was created by a team dedicated to cybersecurity awareness in South Africa." },
                { "why is cybersecurity important", "Cybersecurity is crucial because it protects your personal information, finances, and identity from online threats like hackers and scammers." },
                { "hello", $"Hello {userName}! How can I assist you with cybersecurity today?" },
                { "help", "I'm here to answer your cybersecurity questions. You can ask me about phishing, passwords, or safe browsing." }
            };

            phishingResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "what is phishing", "Phishing is a type of cyber attack where scammers impersonate legitimate organizations via email, text, or fake websites to trick you into revealing sensitive information (passwords, credit card numbers)." },
                { "how to spot phishing email", "Look for: 1) Urgent language threatening account closure. 2) Generic greetings like 'Dear Customer'. 3) Suspicious sender addresses. 4) Spelling/grammar errors. 5) Links that don't match the official website. Hover over links to see the real URL." },
                { "what to do if i clicked a phishing link", "Immediately disconnect from the internet, run a full antivirus scan, change any passwords you entered, enable two-factor authentication, and monitor your accounts for suspicious activity." },
                { "examples of phishing", "Common examples: fake bank emails asking you to verify account details, 'your package is delayed' messages with tracking links, fake lottery winnings, or tech support scams." },
                { "what is smishing", "Smishing is phishing via SMS (text messages). Scammers send texts with malicious links or requests for personal info." },
                { "what is vishing", "Vishing is voice phishing – phone calls where scammers pretend to be from your bank, tech support, or government agencies to extract personal details." },
                { "how to report phishing", "In South Africa, you can report phishing to the South African Police Service (SAPS) cybercrime unit or forward suspicious emails to the company being impersonated (e.g., your bank)." },
                { "what are phishing red flags", "Red flags: unsolicited requests for personal info, mismatched URLs, poor grammar, threats of account closure, and offers that seem too good to be true." }
            };

            passwordResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how to create strong password", "Use a mix of uppercase, lowercase, numbers, and special characters. Aim for at least 12-16 characters. Avoid dictionary words, birthdays, or common patterns. Consider using a passphrase like 'Purple@Elephant#Dance42'." },
                { "what is two factor authentication", "Two-factor authentication (2FA) adds an extra layer of security by requiring not only a password but also a second factor like a code from your phone, fingerprint, or hardware token." },
                { "how often to change passwords", "Change passwords immediately if you suspect a breach. For sensitive accounts, change every 3-6 months. Use a password manager to generate and store unique passwords." },
                { "what is password manager", "A password manager is a tool that generates and stores complex, unique passwords for all your accounts, encrypted behind a single master password." },
                { "should i reuse passwords", "Never reuse passwords across multiple sites. If one site gets hacked, all your accounts become vulnerable." },
                { "how to remember strong passwords", "Use a password manager. Alternatively, create a base phrase and modify it per site (e.g., 'G00gleL0v3r!2024' for Google, 'FbL0v3r!2024' for Facebook) – but a manager is safer." },
                { "what is multi factor authentication", "Multi-factor authentication (MFA) is similar to 2FA but may include more than two factors: something you know (password), something you have (phone), something you are (fingerprint)." },
                { "common password mistakes", "Using 'password123', your name, birthdate, or simple keyboard patterns like 'qwerty'. Also, writing passwords on sticky notes or sharing them via email." }
            };

            safeBrowsingResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how to identify safe websites", "Look for 'https://' in the URL and a padlock icon in the address bar. Be cautious of misspelled domain names (e.g., 'g00gle.com'). Check for contact information and privacy policies." },
                { "what is https", "HTTPS (Hypertext Transfer Protocol Secure) encrypts data between your browser and the website, protecting it from eavesdroppers. Always ensure sites use HTTPS, especially when entering personal info." },
                { "how to avoid fake websites", "Type URLs manually instead of clicking links. Double-check the domain. Use bookmarks for important sites. Avoid clicking on ads that claim 'you've won' or offer unbelievable deals." },
                { "what are cookies safe", "Cookies themselves are just data files. However, third-party tracking cookies can be used to profile your browsing. Use browser settings to block third-party cookies and clear cookies regularly." },
                { "how to browse safely on public wifi", "Avoid accessing sensitive accounts on public Wi-Fi. Use a VPN to encrypt your traffic. Turn off file sharing and ensure your firewall is active. Forget the network after use." },
                { "what is incognito mode", "Incognito/private mode prevents your browser from storing history, cookies, and form data locally. It does NOT make you anonymous online; your ISP and websites can still see you." },
                { "how to check if link is safe", "Hover over the link to see the real URL. Use link scanners like VirusTotal. If unsure, don't click – manually navigate to the site." },
                { "what is browser security", "Browser security includes keeping your browser updated, using security extensions (ad-blockers, anti-tracking), enabling pop-up blockers, and avoiding suspicious downloads." }
            };
        }

        /// <summary>
        /// Starts the main interaction loop.
        /// </summary>
        public void Start()
        {
            while (isRunning)
            {
                DisplayMainMenu();
                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        HandleTopic("Phishing", phishingResponses);
                        break;
                    case "2":
                        HandleTopic("Password Safety", passwordResponses);
                        break;
                    case "3":
                        HandleTopic("Safe Browsing", safeBrowsingResponses);
                        break;
                    case "4":
                        HandleGeneralQuestions();
                        break;
                    case "5":
                        isRunning = false;
                        break;
                    default:
                        ConsoleHelper.WriteColored("Invalid option. Please enter a number from 1 to 5.\n", ConsoleColor.Red);
                        ConsoleHelper.PrintSeparator();
                        break;
                }
            }
        }

        /// <summary>
        /// Displays the main menu.
        /// </summary>
        private void DisplayMainMenu()
        {
            ConsoleHelper.PrintHeader("CYBERSECURITY AWARENESS CHATBOT");
            ConsoleHelper.WriteColored($"Welcome back, {userName}!\n", ConsoleColor.Green);
            ConsoleHelper.WriteColored("Please select an option:\n", ConsoleColor.Cyan);
            ConsoleHelper.PrintMenuItem("1", "Ask about Phishing");
            ConsoleHelper.PrintMenuItem("2", "Ask about Password Safety");
            ConsoleHelper.PrintMenuItem("3", "Ask about Safe Browsing");
            ConsoleHelper.PrintMenuItem("4", "General Questions (How are you? etc.)");
            ConsoleHelper.PrintMenuItem("5", "Exit");
            ConsoleHelper.PrintSeparator();
            Console.Write("Your choice: ");
        }

        /// <summary>
        /// Handles a specific topic (phishing, passwords, safe browsing) by allowing the user to ask questions.
        /// </summary>
        private void HandleTopic(string topicName, Dictionary<string, string> responses)
        {
            bool inTopic = true;
            while (inTopic)
            {
                ConsoleHelper.PrintHeader($"{topicName} QUESTIONS");
                ConsoleHelper.WriteColored($"You can ask me about the following (or type 'back' to return to main menu):\n", ConsoleColor.Yellow);

                // Display sample questions
                int count = 1;
                foreach (var key in responses.Keys.Take(7)) // Show at least 7 sample questions
                {
                    ConsoleHelper.WriteColored($"  {count}. ", ConsoleColor.Yellow);
                    Console.WriteLine(char.ToUpper(key[0]) + key.Substring(1)); // Capitalize first letter
                    count++;
                }
                ConsoleHelper.PrintSeparator('-');
                Console.Write($"{userName}, what would you like to ask? ");

                string question = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(question))
                {
                    ConsoleHelper.WriteColored("You didn't enter anything. Please ask a question or type 'back'.\n", ConsoleColor.Red);
                    continue;
                }

                if (question.Equals("back", StringComparison.OrdinalIgnoreCase))
                {
                    inTopic = false;
                    continue;
                }

                // Try to find a matching response
                string response = GetResponse(question, responses);
                if (response != null)
                {
                    ConsoleHelper.WriteColored("\n🤖 ", ConsoleColor.Cyan);
                    ConsoleHelper.WriteLineWithDelay(response, 20); // Typing effect
                }
                else
                {
                    ConsoleHelper.WriteColored("\n🤖 I didn't quite understand that. Could you rephrase or ask another question? (Type 'back' to return)\n", ConsoleColor.Yellow);
                }
                ConsoleHelper.PrintSeparator();
            }
        }

        /// <summary>
        /// Handles general questions (how are you, purpose, etc.).
        /// </summary>
        /// <summary>
        /// Handles general questions (how are you, purpose, etc.).
        /// </summary>
        private void HandleGeneralQuestions()
        {
            bool inGeneral = true;
            while (inGeneral)
            {
                ConsoleHelper.PrintHeader("GENERAL QUESTIONS");
                ConsoleHelper.WriteColored("You can ask me things like:\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • How are you?\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • What is your purpose?\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • What can I ask you about?\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • Who created you?\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • Why is cybersecurity important?\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • Hello\n", ConsoleColor.Yellow);
                ConsoleHelper.WriteColored("  • Help\n", ConsoleColor.Yellow);


                ConsoleHelper.WriteColored("\n(Type 'back' at any time to return to the main menu)\n", ConsoleColor.Magenta);

                ConsoleHelper.PrintSeparator('-');
                Console.Write($"{userName}, what would you like to ask? ");

                string question = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(question))
                {
                    ConsoleHelper.WriteColored("You didn't enter anything. Please ask a question or type 'back'.\n", ConsoleColor.Red);
                    continue;
                }

                if (question.Equals("back", StringComparison.OrdinalIgnoreCase))
                {
                    inGeneral = false;
                    continue;
                }

                string response = GetResponse(question, generalResponses);
                if (response != null)
                {
                    ConsoleHelper.WriteColored("\n🤖 ", ConsoleColor.Cyan);
                    ConsoleHelper.WriteLineWithDelay(response, 20);
                }
                else
                {
                    ConsoleHelper.WriteColored("\n🤖 I didn't quite understand that. Could you rephrase? (Type 'back' to return)\n", ConsoleColor.Yellow);
                }
                ConsoleHelper.PrintSeparator();
            }
        }

        /// <summary>
        /// Attempts to match user input with a predefined question and returns the corresponding answer.
        /// </summary>
        private string GetResponse(string userInput, Dictionary<string, string> responses)
        {
            // Normalize input: lowercase, trim extra spaces
            string normalized = userInput.ToLower().Trim();

            // Try exact match first
            if (responses.ContainsKey(normalized))
                return responses[normalized];

            // Try partial match: if user input contains any of the keys (simple fallback)
            foreach (var key in responses.Keys)
            {
                if (normalized.Contains(key))
                    return responses[key];
            }

            return null; // No match found
        }
    }
}