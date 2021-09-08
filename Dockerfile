FROM 171861607364.dkr.ecr.sa-east-1.amazonaws.com/superdigital/icu-aspnet:2.2
WORKDIR /app

RUN mkdir ./Super.Antifraude.Whitelist.Consumer

COPY Super.Antifraude.Whitelist.Consumer ./Super.Antifraude.Whitelist.Consumer

WORKDIR /app/Super.Antifraude.Whitelist.Consumer
EXPOSE 80
ENTRYPOINT ["/app/entrypoint.sh", "Super.Antifraude.Whitelist.Consumer.dll"]
