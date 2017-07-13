# Introdução

# Olos Bot Gateway

O Olos Bot Gateway é um webApp baseado no [Microsoft Bot Framework](https://docs.microsoft.com/en-us/bot-framework/#pivot=main&panel=overview) is a standalone Node.js application that uses Express to expose a REST API to the *Olos Bot Gateway* for receiving user messages and addiding them to a message queue. It replies immediately with a *202 ACCEPTED* HTTP status. The messages are then processed asynchronously by the *Olos Bot Engine*, which is a separated Node.js application. Both uses the *queuemanager* module. The former uses it to add user messages to the queue and the latter uses it to read the messages from the queue via events.

The *queuemanager* module has been developed to decouple the chat bot logic from the underlying message queue logic. It uses the [rsmq-worker](https://github.com/mpneuried/rsmq-worker) module which uses the [RSMQ](https://www.npmjs.com/package/rsmq) module to add and consume messages from a
[Redis](https://redis.io) server.
![Architecture Overview](documentation/olos-bot-architecture-overview.png)


# Deploying Olos Bot Gateway 

## Premissas

- Windows Server 2012 is up and running
- IIS 8.5 is up and running
- You have Administrator privileges

## Dependências

Download and install the following software in your Windows server:

| Software    | Version      | URL                                                 | Notes                                        |
| ----------- | -------------| --------------------------------------------------- | -------------------------------------------- |
| Node.js     | (latest)     | [https://nodejs.org/](https://nodejs.org/)                                 | The latest version as of 2017-06-23 is 8.1.2 |
| URL Rewrite | (latest)     | [https://www.iis.net/downloads/microsoft/url-rewrite](https://www.iis.net/downloads/microsoft/url-rewrite) | This is an IIS extension provided by Microsoft. The latest version as of 2017-06-23 is 2.0 |
| iisnode     | (latest)     | [https://github.com/tjanczuk/iisnode](https://github.com/tjanczuk/iisnode) | This is a native IIS 7/8 module for running Node.js applications in it | 


## Publicando o Olos Bot Gateway

In the server that is running IIS, create a directory named `OlosBotReceiver` under the existing directory `C:\inetpub\wwwroot\`;

Copy the contents of the olos-bot-receiver directory to the directory you have created;

Via command line, cd to that directory and execute `npm install`;

>**Note**
>If you don't have access to the Internet from this server, you can execute this step from a different server and then transfer
>the node_modules to the target server;

Give Modify permission to the IIS_IUSRS group of users in the `OlosBotReceiver` directory;

Open the IIS Manager and create a new web site;

Name it ``Olos Bot Receiver``;

Define the physical directory as the `OlosBotReceiver`;

Define a port (example 8080);

>**Note**
>The port you selected will be automatically passed to the Node.js application as a pipe;

Right-click on the web site, Manage Website, Advanced Settings;

Set the Preload Enabled flag to True;

Go to Application Pools, right-click the `Olos Bot Receiver` application pool and then Advanced Settings;

Set the Start Mode to AlwaysRunning;

Set the Maximum Worker Process to the number of cores available to the application (example: 4);

> **Note**
> iisnode is configured to start only one Node.js process (`nodeProcessCountPerApplication="1"`),
> so, considering the example above, there will be a maximum of 4 x 1 = 4 Node.js processes running.
> Please notice IIS will spawn only one IIS worker process when it first starts. It will spawn additional
> IIS worker processes (limited to 4 in the exmaple) only under load (not sure about the thresholds though). 

**External References**

[https://tomasz.janczuk.org/2013/07/application-initialization-of-nodejs.html](https://tomasz.janczuk.org/2013/07/application-initialization-of-nodejs.html)


## Configuração

### Configure o web.config

Currently the hostname/IP and port of the Redis server that the solution connects to are configured in the ``package.json`` file,
so you need to edit this file to make sure it is pointing to the correct Redis hostname/IP and port. Also, other lower level
configuration like pooling interval and retry parameters are configured in that file. Example:

```
  "queue": {
    "host": "10.15.15.41",
    "port": "6379",
    "ns": "rsmq",
    "qname": "OlosBotMainQueue",
    "poolingInterval": [0,1],
    "retryDelay": 5,
    "retryLimit": 8640
  },
  "receiverConfig": {
    "httpPort": 8081,
    "successHttpStatus": 202,
    "successMessage": "OK"
  }
```

As per the above example, in case Redis is unavailable, the solution will retry to connect 8640 times, waiting 5 seconds between each retry.
It will give up after 8640 * 5 = 43200 seconds = 12 hours. In future releases, this configuration is supposed to be read from the main Olos database.

> *Note* The `httpPort` configuration is ignored when the solution is running within iisnode.
> The port configured via the IIS GUI is used instead.

Edit the `web.config` file and make sure it has the following environment variables correctly set:

```
    <appSettings>
        <add key="DEBUG" value=""/>
        <add key="NODE_ENV" value="production"/>
    </appSettings>
```

For a complete list of configration parameters available to the iisnode module, please refer to this
sample file: [https://github.com/tjanczuk/iisnode/blob/master/src/samples/configuration/web.config](https://github.com/tjanczuk/iisnode/blob/master/src/samples/configuration/web.config)



# Observações

## Deploying an Express-based Node.js application in IIS
Besides the instructions from [http://www.galaco.me/node-js-on-windows-server-2012/](http://www.galaco.me/node-js-on-windows-server-2012/)
you shall notice the following:

**bin/www**: by default the main .js file created by the Express Generator is the bin/www. Rename it to `server.js` and move it
to the root of your application to make it simpler to work with IIS. 

**web.config**: make sure the "StaticContent" rule (the one containing `<action type="Rewrite" url="public{REQUEST_URI}"/>`) comes **before**
the "DynamicContent" (the one containing `<action type="Rewrite" url="server.js"/>`


## Debugging
The Olos Bot Receiver uses the [debug](https://www.npmjs.com/package/debug) module, so you can set
the DEBUG environment variable to see debug messages from the different parts of the solution in the console.
Examples of DEBUG environment variable:

```
DEBUG=queuemanager,api
DEBUG=*,-express*,-body-parser*
DEBUG=*
```
Please notice debug messages won't appear in the Visual Studio Code debug console (I don't know why).
