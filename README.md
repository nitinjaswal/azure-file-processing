# File processing on Azure with Blob

This is a cloud-native file processing system built with :
1. ASP.NET Core Web API
2. Azure Blob Storage
3. Azure Functions 
4. Azure SQL Database

It enables users to upload a CSV file via an API, automatically triggers processing through an Azure Function, saves the data into a SQL database, and sends a confirmation email. API is deployed in Azure App Service

# Features
1. Upload CSV file with API
2. Stored uploaded file in Blob storage container
3. Trigger azure function the moment file is uploaded to blob
4. Parse CSV & store the CSV data in Azuew SQL table
5. Send email after the processing

[API: Upload csv file] -> [Azure Blob storage] -> [Blob trigger function] -> [Parse CSV and Insert data into SQL] -> [Send email after processing using send grid]

# Azure setup/resources

1. Azure App Service (Deployed API)
2. Azure Blob Storage
3. Azure Function APP
4. Azure SQL Database
5. Use local.settings.json for Azure Function
6. Use appsettins.json for Web API

# Blob
--It is object storage solution for the cloud, which is optimized for stroing massive amounts of unstructured  data. Unstructured data is data that doesn't adhere to a particular data model or definition, such as text or binary data.

# Blob storage resources
1. The storage account
2. Container in the storage account
3. A blob in a container

# Storage 
1. A storage account provides a unique namespace in Azure for your data. Every object that you store in Azure Storage has an address that includes your unique account name. The combination of the account name and the Blob Storage endpoint forms the base address for the objects in your storage account.

# Containers
1. A container organizes a set of blobs, similar to a directory in a file system. A storage account can include an unlimited number of containers, and a container can store an unlimited number of blobs.


# Resources
1. https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-introduction
2. https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-portal