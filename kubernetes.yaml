apiVersion: apps/v1
kind: Deployment
metadata:
  name: pubsub
  labels:
    app: pubsub
spec:
  replicas: 1
  selector:
    matchLabels:
      app: pubsub
  template:
    metadata:
      labels:
        app: pubsub
    spec:
      containers:
      - name: pubsub
        image: gcr.io/dotnetcore-310104/pubsub:COMMIT_SHA
        ports:
        - containerPort: 8080
---
kind: Service
apiVersion: v1
metadata:
  name: pubsub
spec:
  selector:
    app: pubsub
  ports:
  - protocol: TCP
    port: 80
    targetPort: 8080
  type: LoadBalancer
