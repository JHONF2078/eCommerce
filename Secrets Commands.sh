# Convert text into base64
[Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('guest'))

# Apply secret manifest
kubectl apply -f rabbitmq.secret.yaml

