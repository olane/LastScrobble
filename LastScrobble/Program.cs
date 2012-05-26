using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Xml;

namespace LastScrobble
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a last.fm username.");
                return;
            }

            StringBuilder sb = new StringBuilder();

            byte[] buf = new byte[8192];
            String user = args[0];
            String url = "http://ws.audioscrobbler.com/2.0/user/" + user + "/recenttracks.xml";

            try
            {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();

                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;



                do
                {
                    count = resStream.Read(buf, 0, buf.Length);

                    if (count != 0)
                    {
                        tempString = Encoding.ASCII.GetString(buf, 0, count);
                        sb.Append(tempString);
                    }

                }
                while (count > 0);

                String xml = sb.ToString();



                using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
                {
                    bool foundStuff = false;

                    while (reader.ReadToFollowing("track"))
                    {

                        reader.MoveToFirstAttribute();
                        bool nowplaying = reader.Value == "true";


                        reader.ReadToFollowing("artist");
                        String artist = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("name");
                        String name = reader.ReadElementContentAsString();

                        int padding = 35 - name.Length;

                        for (int i = 0; i < padding; i++)
                        {
                            //I'm a terrible person.
                            name += " ";
                        }

                        if (name.Length > 40)
                        {
                            name = name.Substring(0, 35);
                        }

                        String output = name + " -  " + artist;

                        if (nowplaying)
                        {
                            output += "   (Now playing)";
                        }

                        Console.WriteLine(output);

                        foundStuff = true;

                    }

                    if (!foundStuff)
                    {
                        Console.WriteLine("Sorry, didn't find any data. Was the username valid?");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}
