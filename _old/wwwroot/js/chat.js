// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";

let con = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
let sendBtn = document.getElementById("sendButton");
let loggedUserId = parseInt(document.getElementById("userId").value);
let chat = document.getElementsByClassName("col-md-9")[0];

sendBtn.disabled = true;
chat.style.display = "none !important";

function addMessage(senderId, loggedUserId, senderNickname, message, date) {
    const li = document.createElement("li");
    li.classList.add(senderId === loggedUserId ? "message-right" : "message-left");

    li.innerHTML = `
        <div class="message-box">
            <div class="sender-name">${senderNickname}</div>
            <div class="message-content">${message}</div>
            <div class="message-date">Wysłano: ${new Date(date).toLocaleString()}</div>
        </div>
    `;

    document.getElementById("messageList").appendChild(li);
}

function loadMessages(userId) {
    const xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && this.status === 200) {
            const messages = JSON.parse(this.responseText);
            const messageList = document.getElementById("messageList");
            messageList.innerHTML = "";

            messages.forEach(m => {
                addMessage(m.senderId, loggedUserId, m.senderNickname, m.content, m.sentAt)
            });
        }
    };
    xhttp.open("GET", `/Messages/GetMessages?userId=${userId}`, true);
    xhttp.send();
}

document.querySelectorAll(".chat-user").forEach(li => {
    li.addEventListener("click", () => {
        let selectedUserId = parseInt(li.getAttribute("data-user-id"));
        let selectedConversationId = parseInt(li.getAttribute("data-conversation-id"));
        document.getElementById("selectedUserId").value = selectedUserId;
        document.getElementById("selectedConversationId").value = selectedConversationId;
        document.getElementById("receiverNickname").innerText = li.innerText;
        chat.style.display = "flex !important";
        loadMessages(selectedUserId);
    });
});

con.on("ReceiveMessage", (senderId, conversationId, senderNickname, message, sentAt) => {
    const activeChat = parseInt(document.getElementById("selectedConversationId").value);
    if (conversationId === activeChat) {
        addMessage(senderId, loggedUserId, senderNickname, message, sentAt);
    }
});

con.on("NewConversation", (user) => {
    const li = document.createElement("li");
    li.classList.add("list-group-item", "chat-user");

    li.dataset.userId = user.userId;
    li.textContent = user.userName;

    li.addEventListener("click", () => {
        document.getElementById("selectedUserId").value = user.userId;
        document.getElementById("selectedConversationId").value = user.conversationId;
        loadMessages(user.userId);
    });

    document.getElementById("userList").appendChild(li);
});

con.start().then(() => {
    sendBtn.disabled = false;
}).catch((ex) => {
    return console.error(ex.toString());
});

function send(e){
    e.preventDefault();

    let receiverUser = parseInt(document.getElementById("selectedUserId").value);
    let message = document.getElementById("messageInput").value;

    if(!receiverUser || message.trim() === "") return;

    con.invoke("SendMessage", receiverUser, message).catch(function (ex) { return console.error(ex.toString()) });
    document.getElementById("messageInput").value = "";
    setTimeout(() => {
        document.getElementById("messages").scrollTop = document.getElementById("messages").scrollHeight;
    }, 500)    
}

sendBtn.addEventListener("click", (e) => {
    send(e);
});

document.getElementById("messageInput").addEventListener("keyup", (e) => {
    if(e.key === "Enter"){
        send(e);
    }
});