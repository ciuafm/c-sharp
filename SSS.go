package main

import (
    _ "github.com/denisenkom/go-mssqldb"
    "database/sql"
    "context"
    "log"
    "fmt"
)

var db *sql.DB

var server = "uali.database.windows.net"
var port = 1433
var user = "Alex"
var password = "Beer4321"
var database = "books-index"

func main() {
    // Build connection string
    connString := fmt.Sprintf("server=%s;user id=%s;password=%s;port=%d;database=%s;",
        server, user, password, port, database)

    var err error

    // Create connection pool
    db, err = sql.Open("sqlserver", connString)
    if err != nil {
        log.Fatal("Error creating connection pool:", err.Error())
    }
    fmt.Printf("Connected!\n")

    createId, err := InsertLogRecord("22542","87820","59107","8591;19801;19815;19800;19812","55;64;63;31;56;45;62;30","6680;13176;13196;13306;13364","2018-03-02 10:06:52.000")
    fmt.Printf("Inserted ID: %d successfully.\n", createId)
    
}

func InsertLogRecord(Uptime string,TotalHR string,TotalSH string,HRpC string,TCpC string,SHpC string,time string) (int64, error) {
    ctx := context.Background()
    var err error

    if db == nil {
        log.Fatal("What?")
    }

    // Check if database is alive.
    err = db.PingContext(ctx)
    if err != nil {
        log.Fatal("Error pinging database: " + err.Error())
    }

    tsql := fmt.Sprintf("INSERT INTO [dbo].[Miner_log] ( [Uptime] ,[TotalHR],[TotalSH],[HRpC],[TCpC],[SHpC],[time]) VALUES (@Uptime,@TotalHR,@TotalSH,@HRpC,@TCpC,@SHpC,@time);")
	
    // Execute non-query with named parameters
    result, err := db.ExecContext(
        ctx,
        tsql,
		sql.Named("Uptime", Uptime),
		sql.Named("TotalHR", TotalHR),
		sql.Named("TotalSH", TotalSH),
		sql.Named("HRpC", HRpC),
		sql.Named("TCpC", TCpC),
		sql.Named("SHpC", SHpC),
		sql.Named("time", time))

    if err != nil {
        log.Fatal("Error inserting new row: " + err.Error())
        return -1, err
    }

    return result.LastInsertId()
}
