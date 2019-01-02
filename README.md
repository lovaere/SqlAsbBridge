# SqlAsbBridge PoC
This SqlAsbBridge project is a small proof-of-concept in order to validate the communication between two '[NServiceBus](https://particular.net/nservicebus) islands' with the use of the [NServiceBus.Router](https://github.com/SzymonPobiega/NServiceBus.Router) package. With 'islands' We mean two environments with a [different transport](https://docs.particular.net/transports/) that are normally not able to communicate with each other via NServiceBus.

## Azure Service Bus Island
The first island uses the [Azure Service Bus transport](https://docs.particular.net/transports/azure-service-bus/). The **SqlAsbBridge.AsbPublisher** console application will publish/send the following messages on startup:
* ```AzureActionCompletedEvent```
* ```StartAzureActionCommand```

And these are handled by the **SqlAsbBridge.AsbHandler** project. All this happens on the ASB island, except for the ```SqlActionCompletedEventHandler``` that originates from the SQL island.

## SQL Server Island
The second island uses the [SQL-Server transport](https://docs.particular.net/transports/sql/) layer. Here we have a more or less identical setup. The **SqlAsbBridge.SqlPublisher** console application will publish/send the following messages on startup:
* ```SqlActionCompletedEvent```
* ```StartSqlActionCommand```

And these are handled by the **SqlAsbBridge.SqlHandler** project. In this case this all happens on the SQL-server transport, except for the ```AzureActionCompletedEventHandler``` that originates from the Azure Service Bus island.