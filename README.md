# DestreamerChannelDL

Very simple program to bulk download videos from microsoft stream channel.

Requirements:
.NET core runtime, https://dotnet.microsoft.com/download
A built destreamer

Preparation:
1) Open up the channel you'd like to download videos from
2) Scroll to the bottom to load all the videos
3) Right click > inspect > copy the body and save it to a file somewhere

Usage:
streamchannelDL.exe "path-to-file" "path-to-dstreamer.cmd" "output-dir" [--threads X]
The threads setting is optional, it lets you parallelize the download (but in a very janky way that's essentially just running multiple destreamer instances).

I made this in like an afternoon, might have bugs idk. The "isdev" check simply autopopulates it with my own settings because I'm lazy AF.



Uh. That's about it. Why are you still here? Leave.
