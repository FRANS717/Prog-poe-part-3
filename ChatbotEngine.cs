using System;
using System.Collections.Generic;

namespace PROG6221_V1
{
    public delegate string BotResponseDelegate(string input);

    public class ChatbotEngine
    {
        private string userName;
        private string favouriteTopic = "";
        private string currentTopic = "";

        private readonly Random random = new Random();

        private readonly Dictionary<string, string> generalResponses;
        private readonly Dictionary<string, string> phishingResponses;
        private readonly Dictionary<string, string> passwordResponses;
        private readonly Dictionary<string, string> safeBrowsingResponses;

        private readonly Dictionary<string, List<string>> randomResponses;
        private readonly Dictionary<string, List<string>> sentimentResponses;

        public ChatbotEngine(string userName)
        {
            this.userName = string.IsNullOrWhiteSpace(userName) ? "Friend" : userName;

            generalResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how are you", $"I'm just a program, {this.userName}, but I'm ready to help you stay safe online." },
                { "what is your purpose", $"My purpose is to educate you about cybersecurity, {this.userName}. I can answer questions about phishing, password safety, and safe browsing." },
                { "what can i ask you about", "You can ask me about phishing emails, scams, password safety, safe browsing habits, privacy, 2FA, and general cybersecurity awareness." },
                { "who created you", "I was created as part of a cybersecurity awareness project for South African citizens." },
                { "why is cybersecurity important", "Cybersecurity protects your personal information, finances, and identity from online threats like hackers and scammers." },
                { "hello", $"Hello {this.userName}! How can I assist you with cybersecurity today?" },
                { "help", "Ask me about phishing, passwords, safe browsing, scams, privacy, or 2FA." }
            };

            phishingResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "what is phishing", "Phishing is a cyber attack where scammers impersonate trusted organisations through email, SMS, calls, or fake websites to steal sensitive information." },
                { "how to spot phishing email", "Look for urgent language, generic greetings, suspicious sender addresses, spelling errors, and links that do not match official websites." },
                { "what to do if i clicked a phishing link", "Disconnect from the internet, run an antivirus scan, change affected passwords, enable 2FA, and monitor your accounts." },
                { "examples of phishing", "Examples include fake bank emails, fake delivery messages, fake lottery winnings, and fake tech support warnings." },
                { "what is smishing", "Smishing is phishing through SMS messages. Scammers send malicious links or fake alerts by text." },
                { "what is vishing", "Vishing is voice phishing where scammers call pretending to be from banks, tech support, or government services." },
                { "how to report phishing", "Report phishing to the organisation being impersonated or the relevant cybercrime authority." },
                { "what are phishing red flags", "Red flags include urgency, poor grammar, mismatched links, requests for personal details, and unrealistic offers." }
            };

            passwordResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how to create strong password", "Use uppercase letters, lowercase letters, numbers, and special characters. Aim for at least 12-16 characters." },
                { "what is two factor authentication", "Two-factor authentication adds a second security step, such as a code from your phone or authenticator app." },
                { "how often to change passwords", "Change passwords immediately after a breach. For sensitive accounts, update them regularly and keep them unique." },
                { "what is password manager", "A password manager stores and generates strong unique passwords behind one master password." },
                { "should i reuse passwords", "Never reuse passwords. If one site is hacked, every reused password becomes a weakness." },
                { "how to remember strong passwords", "Use a password manager or create long passphrases that are memorable but hard to guess." },
                { "what is multi factor authentication", "MFA uses multiple identity checks, such as something you know, something you have, or something you are." },
                { "common password mistakes", "Common mistakes include password123, birthdays, names, qwerty, sticky notes, and sharing passwords." }
            };

            safeBrowsingResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "how to identify safe websites", "Look for HTTPS, check the domain spelling, avoid suspicious pop-ups, and make sure the site looks trustworthy." },
                { "what is https", "HTTPS encrypts data between your browser and the website, helping protect your information." },
                { "how to avoid fake websites", "Type URLs manually, use bookmarks, double-check domains, and avoid suspicious adverts." },
                { "what are cookies safe", "Cookies are usually harmless, but tracking cookies can monitor browsing. Clear them and block third-party cookies where possible." },
                { "how to browse safely on public wifi", "Avoid sensitive logins on public Wi-Fi. Use a VPN, disable file sharing, and forget the network after use." },
                { "what is incognito mode", "Incognito mode hides local history, but it does not make you anonymous online." },
                { "how to check if link is safe", "Hover over links, inspect the URL, use link scanners, and avoid links that look strange." },
                { "what is browser security", "Browser security includes updates, pop-up blocking, anti-tracking tools, and avoiding suspicious downloads." }
            };

            randomResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "phishing", new List<string> { phishingResponses["what is phishing"], phishingResponses["how to spot phishing email"], phishingResponses["what are phishing red flags"] } },
                { "password", new List<string> { passwordResponses["how to create strong password"], passwordResponses["what is password manager"], passwordResponses["should i reuse passwords"] } },
                { "safe browsing", new List<string> { safeBrowsingResponses["how to identify safe websites"], safeBrowsingResponses["how to avoid fake websites"], safeBrowsingResponses["how to browse safely on public wifi"] } },
                { "scam", new List<string> { "Scams pressure you to act fast. Slow down and verify.", "Never share OTPs, passwords, or banking details unexpectedly.", "If an offer feels too good to be true, it probably is." } },
                { "privacy", new List<string> { "Privacy means controlling what information you share online.", "Review social media privacy settings often.", "Avoid posting your ID number, address, live location, or routine." } },
                { "2fa", new List<string> { passwordResponses["what is two factor authentication"], "2FA protects your account even if your password is stolen.", "Authenticator apps are usually safer than SMS codes." } }
            };

            sentimentResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "worried", new List<string> { "It makes sense to feel worried. Start with strong passwords, 2FA, and avoiding suspicious links.", "Do not panic. Cybersecurity improves one habit at a time." } },
                { "frustrated", new List<string> { "I get it. Cybersecurity can be annoying, but protecting your accounts is worth it.", "Let us simplify this and focus on one safety step." } },
                { "curious", new List<string> { "Good. Curiosity makes you harder to trick.", "Ask about phishing, passwords, scams, privacy, safe browsing, or 2FA." } }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something so I can help you.";

            input = input.ToLower().Trim();

            BotResponseDelegate sentimentChecker = DetectSentiment;
            string sentiment = sentimentChecker(input);

            string memory = DetectMemory(input);
            if (!string.IsNullOrWhiteSpace(memory))
                return memory;

            string exact = DetectOriginalResponse(input);
            string topic = DetectTopic(input);

            if (!string.IsNullOrWhiteSpace(sentiment) && !string.IsNullOrWhiteSpace(topic))
                return sentiment + Environment.NewLine + Environment.NewLine + topic;

            if (!string.IsNullOrWhiteSpace(exact))
                return exact;

            if (!string.IsNullOrWhiteSpace(sentiment))
                return sentiment;

            if (!string.IsNullOrWhiteSpace(topic))
                return topic;

            if (input.Contains("tell me more") || input.Contains("another tip") || input.Contains("explain more"))
                return GiveFollowUpResponse();

            return "I didn’t quite understand that. Could you rephrase?";
        }

        private string DetectOriginalResponse(string input)
        {
            string response;

            response = MatchDictionary(input, generalResponses);
            if (response != null) return response;

            response = MatchDictionary(input, phishingResponses);
            if (response != null) { currentTopic = "phishing"; return response; }

            response = MatchDictionary(input, passwordResponses);
            if (response != null) { currentTopic = "password"; return response; }

            response = MatchDictionary(input, safeBrowsingResponses);
            if (response != null) { currentTopic = "safe browsing"; return response; }

            return "";
        }

        private string DetectTopic(string input)
        {
            foreach (string topic in randomResponses.Keys)
            {
                if (input.Contains(topic))
                {
                    currentTopic = topic;
                    return GetRandomResponse(randomResponses[topic]);
                }
            }

            return "";
        }

        private string DetectSentiment(string input)
        {
            foreach (string feeling in sentimentResponses.Keys)
            {
                if (input.Contains(feeling))
                    return GetRandomResponse(sentimentResponses[feeling]);
            }

            return "";
        }

        private string DetectMemory(string input)
        {
            if (input.Contains("my name is"))
            {
                userName = input.Replace("my name is", "").Trim();

                if (string.IsNullOrWhiteSpace(userName))
                    userName = "Friend";

                return "Got it. I’ll call you " + userName + ".";
            }

            if (input.Contains("interested in"))
            {
                foreach (string topic in randomResponses.Keys)
                {
                    if (input.Contains(topic))
                    {
                        favouriteTopic = topic;
                        currentTopic = topic;
                        return "Great, " + userName + ". I’ll remember that you are interested in " + favouriteTopic + ".";
                    }
                }
            }

            if (input.Contains("remember") || input.Contains("what do you know about me"))
            {
                if (!string.IsNullOrWhiteSpace(favouriteTopic))
                    return "I remember that your name is " + userName + " and you are interested in " + favouriteTopic + ".";

                return "I remember that your name is " + userName + ".";
            }

            return "";
        }

        private string GiveFollowUpResponse()
        {
            if (string.IsNullOrWhiteSpace(currentTopic))
                return "Tell me which topic you want more information about: phishing, password, privacy, scam, safe browsing, or 2FA.";

            if (randomResponses.ContainsKey(currentTopic))
                return GetRandomResponse(randomResponses[currentTopic]);

            return "Ask me about phishing, password safety, privacy, scams, safe browsing, or 2FA.";
        }

        private string MatchDictionary(string input, Dictionary<string, string> responses)
        {
            if (responses.ContainsKey(input))
                return responses[input];

            foreach (string key in responses.Keys)
            {
                if (input.Contains(key))
                    return responses[key];
            }

            return null;
        }

        private string GetRandomResponse(List<string> responses)
        {
            return responses[random.Next(responses.Count)];
        }

        public string GetAsciiArt()
        {
            return AsciiArt.GetArt();
        }
    }
}