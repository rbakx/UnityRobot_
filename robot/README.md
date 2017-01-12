#Robot ([brokers])
Unity communicates with a robot via a broker. The broker is responsible for translating general commands into robot specific commands. This means each robot type will need its own broker type.

##EV3 robot
The EV3 robot receives commands from the broker, and performs action(s) based on the command(s). All commands need to be implemented on the program running on the EV3.

The EV3 robot needs an usb wifi dongle to connect to the broker.

###Messageboxes
Communication on the EV3 uses messageboxes. All sending is done via one messagebox, receiving can be done on multiple message boxes which eliminates the need for parsing the message type.

More information about the EV3 messageboxes here:
- [EV3 firmware developer kit]
- [EV3 direct command documentation]
- Sending data over WiFi between our PC application and the EV3 (Sioux blog post)
  - [part 1]
  - [part 2]
  - [part 3]
  - [part 4]

##EV3 broker
When started the broker will listen for a upd broadcast from an EV3. When this is received the broker will respond to it, which signals the EV3 to enable tcp connection. The address of the EV3 can also be derived from this broadcast. When an EV3 is found, a tcp connection is setup. A tcp connection with unity will also be setup. 

[brokers]: https://en.wikipedia.org/wiki/Message_broker

[EV3 firmware developer kit]: https://le-www-live-s.legocdn.com/sc/media/files/ev3-developer-kit/lego%20mindstorms%20ev3%20firmware%20developer%20kit-7be073548547d99f7df59ddfd57c0088.pdf?la=en-us
[EV3 direct command documentation]: http://cache.lego.com/r/www/r/mindstorms/-/media/franchises/mindstorms%202014/downloads/firmware%20and%20software/advanced/lego%20mindstorms%20ev3%20communication%20developer%20kit.pdf?l.r2=1239680513

[part 1]: https://siouxnetontrack.wordpress.com/2014/08/19/sending-data-over-wifi-between-our-pc-application-and-the-ev3-part-1/
[part 2]: https://siouxnetontrack.wordpress.com/2014/08/21/sending-data-over-wifi-between-our-pc-application-and-the-ev3-part-2/
[part 3]: https://siouxnetontrack.wordpress.com/2014/08/23/sending-data-over-wifi-between-our-pc-application-and-the-ev3-part-3/
[part 4]: https://siouxnetontrack.wordpress.com/2014/08/27/sending-data-over-wifi-between-our-pc-application-and-the-ev3-part-4/
