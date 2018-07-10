package main

import (
	//"bytes"
    "net/url"
	"net/http"
)
func main() {
    //body := []byte("key1=val1&key2=val2")
    //http.Post("http://ual.cloudapp.net", "body/type", bytes.NewBuffer(body))
    // Code to process response (written in Get request snippet) goes here

    // Simulating a form post is done like this:
    http.PostForm("http://ual.cloudapp.net:9090/post",
                            url.Values{"key": {"Value"}, "id": {"123"}})
    // Code to process response (written in Get request snippet) goes here
}

