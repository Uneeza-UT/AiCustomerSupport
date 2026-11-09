window.addEventListener("swagger-ready", async () => {
    let currentUserId = null;
    let token = null;
    let currentConversationId = null;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/support-chat", {
            accessTokenFactory: () => {

                const authorized = window.ui.authSelectors.authorized();
                token = authorized.getIn(["Bearer", "value"]);

                if (!token) {
                    return null;
                }

                const payload = JSON.parse(atob(token.split(".")[1]))

                currentUserId = payload.UserId;

                return token;
            }
        })
        .build();


    function displayMessage(message) {

        const messageElement = document.createElement("div");
        messageElement.classList.add("message");
        messageElement.dataset.messageId = message.id;

        if (message.senderId === message.customerId) {
            if (currentUserId === message.customerId) {
                messageElement.classList.add("message-sent");
            }

            else {
                messageElement.classList.add("message-received");
            }
        }


        else {
            if (currentUserId === message.customerId) {
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



    async function loadMessages(conversationId) {

        document.getElementById("messages").innerHTML = "";

        const response = await fetch(`/api/conversation/${conversationId}/messages`, {
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

        if (message.conversationId !== currentConversationId) {
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
            const conversationId = document.getElementById("conversation-id").value;

            if (!conversationId) {
                console.error("Please enter a conversation ID.");
                return;
            }

            currentConversationId = Number(conversationId);

            await loadMessages(conversationId);
        })    
    
});








