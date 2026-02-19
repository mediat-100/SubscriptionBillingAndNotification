# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SubscriptionBillingAndNotification/SubscriptionBillingAndNotification.csproj", "SubscriptionBillingAndNotification/"]
COPY ["SubscriptionBillingAndNotificationCore/SubscriptionBillingAndNotificationCore.csproj", "SubscriptionBillingAndNotificationCore/"]

RUN dotnet restore SubscriptionBillingAndNotification/SubscriptionBillingAndNotification.csproj

COPY SubscriptionBillingAndNotification/ SubscriptionBillingAndNotification/
COPY SubscriptionBillingAndNotificationCore/ SubscriptionBillingAndNotificationCore/
WORKDIR "/src/SubscriptionBillingAndNotification"
RUN dotnet publish "SubscriptionBillingAndNotification.csproj" -o /published /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /published .
ENTRYPOINT ["dotnet", "SubscriptionBillingAndNotification.dll"]