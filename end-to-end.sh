#!/bin/bash
set -e
# host="http://cicdexample.com/staging/todoapi"
host="localhost:5000"

curl $host/hello

id=$(uuidgen)
echo ""
curl $host/todo \
  -H  "accept: application/json" \
  -H  "Content-Type: application/json" \
  -d "{
      \"id\":\"$id\",
      \"title\":\"comprar\",
      \"url\":\"www.google.com\",
      \"text\":\"comprar cosas\",
      \"completed\":false
    }"

echo " "