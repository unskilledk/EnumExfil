# EnumExfil
Basic enumeration and exfil

Real rough basic enumeration and exfil over DNS, HTTP GET, and HTTP POST. Idea would be for a USB drop, but could probably be used in other cases. Gathers info, hits it with GPG, then changed to hex to exfil. This is only one piece. Requires you setting up a server with DNS and HTTP(S), a script to pull from logs and dehex (unhex?).

I put this together over the course of a few nights with the help of some tequila. Which makes for a fun first experience with C#. Never completed the scripts for the server since we ended up finding the tool we would usually use for that type of engagement.
