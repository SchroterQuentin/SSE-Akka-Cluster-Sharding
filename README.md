# POC for SSE generated from Akka cluster

Run the project with docker compose

```
$ docker compose up -d lighthouse
$ docker compose up -d api
```

Then check the logs of api and run

```
$ docker compose up test
```

You should see number coming from the backend while generated on 3 differents containers. Awesome !