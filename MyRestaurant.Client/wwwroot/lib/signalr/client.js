
   this.connection = new signalR.HubConnectionBuilder()
   .withUrl("https://localhost:44389/menuHub")
   .configureLogging(signalR.LogLevel.Information)
   .build();


    async function start() {
        try{
            await connection.start();
            console.log('connected');

            connection.on("Send", (data) => { 
                console.log("data");
            });
        } catch (err) {
            console.log(err);
            setTimeout(() => start(), 5000);
        }
   
    
    connection.onclose(async () => {
        await start();
    });

    }
    start();





        
