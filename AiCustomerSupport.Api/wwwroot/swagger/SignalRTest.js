window.addEventListener("swagger-ready", async () => {
    let currentUserId = null;
    let token = null;
    let currentTicketId = null;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/support-chat", {
            accessTokenFactory: () => {

                const authorized = window.ui.authSelectors.authorized();
                token = authorized.getIn(["Bearer", "value"]);

                if (!token) {
                    return null;
                }

                const payload = JSON.parse(atob(token.split(".")[1]))

                currentUserId = payload.uid;

                return token;
            }
        })
        .build();


    function displayMessage(message) {

        const messageElement = document.createElement("div");
        messageElement.classList.add("message");
        messageElement.dataset.messageId = message.id;

        if (message.senderId === message.ticket.customerId) {
            if (currentUserId === message.ticket.customerId) {
                messageElement.classList.add("message-sent");
            }

            else {
                messageElement.classList.add("message-received");
            }
        }


        else {
            if (currentUserId === message.ticket.customerId) {
                messageElement.classList.add("message-received");
            }

            else {
                messageElement.classList.add("message-sent");
            }
        }



        if (message.content) {
            const contentElement = document.createElement("div");
            contentElement.textContent = message.content;
            messageElement.appendChild(contentElement);
        }


        if (message.attachments && message.attachments.length > 0) {

            message.attachments.forEach(attachment => {

                const attachmentElement = document.createElement("a");

                attachmentElement.href = `/${attachment.filePath}`;
                attachmentElement.textContent = attachment.fileName;
                attachmentElement.target = "_blank";

                messageElement.appendChild(attachmentElement);
            });
        }

        document.getElementById("messages").appendChild(messageElement);
    }



    async function loadMessages(ticketId) {

        document.getElementById("messages").innerHTML = "";

        const response = await fetch(`/api/ticket/${ticketId}/messages`, {
            headers: {
                Authorization: `Bearer ${token}`
            }
        });


        if (!response.ok) {
            console.error("Failed to load messages.");
            return;
        }

        const messages = await response.json();

        messages.forEach(message => {
            displayMessage(message);
        });
    }

 


    connection.on("ReceiveMessage", message => {

        if (message.ticketId !== currentTicketId) {
            return;
        }

        displayMessage(message);
 
    });



    connection.on("ReceiveNotification", notification => {

        alert("Notification: " + notification.message)
    });


    document.getElementById("connect-signalr")
        .addEventListener("click", async () => {
            try
            {
                if (connection.state !== signalR.HubConnectionState.Disconnected) {
                    await connection.stop();
                }

                document.getElementById("messages").innerHTML = "";

                await connection.start();

                alert("SignalR Connected!");

            }
            catch (error)
            {
                console.error("SignalR connection failed:", error);
            }
        })


    document.getElementById("messages-button")
        .addEventListener("click", async () => {
            const ticketId = document.getElementById("ticket-id").value;

            if (!ticketId) {
                console.error("Please enter a ticket ID.");
                return;
            }

            currentTicketId = Number(ticketId);

            await loadMessages(ticketId);
        })    
    
});








