using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RegExMove
{
    class Program
    {
        static int verbosity;
        static string defRegexStr = @"^(.+?) - (.+)\.(mp3|wav)$";

        static void Main (string[] args)
        {
            bool showHelp = false;

            // get working directory
            string workDir = Directory.GetCurrentDirectory ();
            // get regex move group
            int selector = 1;

            var p = new OptionSet () {
                { "s|selector=", "the regex group index to use for the folder name.\n" +
                    "index 0 is the complete match.\n" +
                    "(default=1).\n" +
                    "this must be an integer.",
                    (int v) => selector = Math.Max (0, v) },
                { "v|verbose", "increase debug message verbosity",
                   (v) => { if (v != null) ++verbosity; } },
                { "h|help",  "show this message and exit",
                   (v) => showHelp = v != null },
            };

            List<string> extra;
            try
            {
                extra = p.Parse (args);
            }
            catch (OptionException e)
            {
                Console.WriteLine (e.Message);
                Console.WriteLine ("Try `RegExMove --help' for more information.");
                return;
            }

            if (showHelp)
            {
                ShowHelp (p);
                return;
            }

            // get regex 
            string regexStr = defRegexStr;
            if (extra.Count > 0)
            {
                regexStr = string.Join (" ", extra.ToArray ());
            }

            // loop files in working dir
            Debug ("Creating regex: '{0}'", regexStr);
            Regex regex = new Regex (regexStr);

            string[] files = Directory.GetFiles (workDir);
            foreach (string file in files)
            {
                string fn = Path.GetFileName (file);
                Debug ("Checking file: '{0}'", fn);

                // apply regex
                Match match = regex.Match (fn);

                // move if enough matches are found to get a dest folder
                if (match.Success && match.Groups.Count > selector)
                {
                    // Finally, we get the Group value and move the file
                    string dn = match.Groups[selector].Value;
                    dn = Path.Combine (Path.GetDirectoryName (file), dn);

                    if (!Directory.Exists (dn))
                    {
                        Debug ("Creating dir: '{0}'", dn);
                        Directory.CreateDirectory (dn);
                    }

                    Debug ("Moving '{0}' to '{1}'", fn, dn);
                    File.Move (file, Path.Combine (dn, fn));
                }
            }
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: RegExMove [OPTIONS] + regex");
            Console.WriteLine ("Moving files matching the given regex into separate folders using one of the matched regex groups.");
            Console.WriteLine ("If no regex is specified, '{0}' is used.", defRegexStr);
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }

        static void Debug (string format, params object[] args)
        {
            if (verbosity > 0)
            {
                Console.Write ("# ");
                Console.WriteLine (format, args);
            }
        }
    }
}
