use tokio::net::TcpListener;
use tokio::io::{AsyncReadExt, AsyncWriteExt};

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    // Bind the server to a local address
    let listener = TcpListener::bind("127.0.0.1:8080").await?;
    println!("Server running on 127.0.0.1:8080");

    loop {
        // Accept incoming connections
        let (mut socket, addr) = listener.accept().await?;
        println!("New connection from {:?}", addr);

        tokio::spawn(async move {
            let mut buffer = [0; 1024];

            // Read data from the socket
            match socket.read(&mut buffer).await {
                Ok(n) if n > 0 => {
                    println!("Received: {}", String::from_utf8_lossy(&buffer[..n]));
                    // Echo back the data
                    if let Err(e) = socket.write_all(&buffer[..n]).await {
                        eprintln!("Failed to write to socket: {}", e);
                    }
                }
                Ok(_) => println!("Connection closed"),
                Err(e) => eprintln!("Failed to read from socket: {}", e),
            }
        });
    }
}