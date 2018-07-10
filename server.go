/*
* Sample API with GET and POST endpoint.
* POST data is converted to string and saved in internal memory.
* GET endpoint returns all strings in an array.
 */
 package main
 
 import (
	 "encoding/json"
	 "flag"
	 "fmt"
	 "io/ioutil"
	 "log"
	 "net/http"
	 "time"
	 "strings"
 )
 
 var (
	 // flagPort is the open port the application listens on
	 flagPort = flag.String("port", "9090", "Port to listen on")
 )
 
 var results []string
 
 // GetHandler handles the index route
 func GetHandler(w http.ResponseWriter, r *http.Request) {
	 jsonBody, err := json.Marshal(results)
	 if err != nil {
		 http.Error(w, "Error converting results to json",
			 http.StatusInternalServerError)
	 }
	 w.Write(jsonBody)
 }
 
 // PostHandler converts post request body to string
 func PostHandler(w http.ResponseWriter, r *http.Request) {
	 if r.Method == "POST" {
		 body, err := ioutil.ReadAll(r.Body)
		 if err != nil {
			 http.Error(w, "Error reading request body",
				 http.StatusInternalServerError)
		 }
		 results = []string{}
		 results =  append(results,time.Now().Format(time.RFC3339))
		 results =  append(results, string(body))
		 s := strings.Split(string(body), ",")
		 var replacer = strings.NewReplacer(" ", "", "\"", "")
		 var replacert = strings.NewReplacer("T", " ", "Z", "")
		 ti := replacert.Replace(time.Now().Format(time.RFC3339))
		 s1 := replacer.Replace(s[1])
		 st1,_ := time.ParseDuration(s1+"m")
		 ss1 := st1.String()
		 ss1 = ss1[:len(ss1)-2]
		 t := strings.Split(replacer.Replace(s[2]), ";")
		 s2 := t[0]
		 s3 := t[1]
		 s4 := replacer.Replace(s[3])
		 s5 := replacer.Replace(s[6])
		 s6 := replacer.Replace(s[9])
		 log.Printf(s1+" | "+ss1+" | "+s2+" | "+s3+" | "+s4+" | "+s5+" | "+s6+" | "+ti)
		 
		 results = []string{}
		 results =  append(results,time.Now().Format(time.RFC3339))
		 results =  append(results, ss1)
		 results =  append(results, s1)
		 results =  append(results, s2)
		 results =  append(results, s3)
		 results =  append(results, s4)
		 results =  append(results, s5)
		 results =  append(results, s6)
		 fmt.Fprint(w, "POST done")
	 } else {
		 http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
	 }
 }
 
 func init() {
	 log.SetFlags(log.Lmicroseconds | log.Lshortfile)
	 flag.Parse()
 }
 
 func main() {
	 results = append(results, time.Now().Format(time.RFC3339))
 
	 mux := http.NewServeMux()
	 mux.HandleFunc("/", GetHandler)
	 mux.HandleFunc("/post", PostHandler)
 
	 log.Printf("listening on port %s", *flagPort)
	 log.Fatal(http.ListenAndServe(":"+*flagPort, mux))
 }