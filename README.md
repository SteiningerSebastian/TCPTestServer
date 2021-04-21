# TCP Test Server
The TcpTestServer is a small tcp server with a simple scripting language.
It should be used for testing tcp-clients implementation and apis.

## The script (example)
```ruby
#example variables
var rec = $RECEIVE
var message = "Hello World"
var who = "who"
var test = "{json: {[data:{index:"hello"}]}}"

#how to respond on each event
if rec=="Hello":
	send("World", rec.id)
	sendToAll(message)
end

if rec==who:
	send(who, rec.id)
	sendToAll("Sebastian")
end

if rec==test:
	sendToAll(test)
end

#This prod. an echo
if rec==rec:
	send(rec, rec.id)
end

#write value into console
if rec==rec:
	log(rec)
end
```
