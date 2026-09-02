# Deploying to the mini PC (DAST target)

Goal: run the vulnerable app on the homelab mini PC so OWASP ZAP can scan it
over the LAN. The app is **only** ever exposed on the home network.

Mini PC: Linux Mint, user `owner`, `192.168.88.13` (Wi-Fi).

## 1. Free up RAM — text-only boot

```bash
sudo systemctl set-default multi-user.target
sudo reboot
```

(Reverse later with `sudo systemctl set-default graphical.target`.)

## 2. Get the code

```bash
git clone https://github.com/charles-goodsir/appsec-homelab.git
cd appsec-homelab
```

(or `git pull` if already cloned)

## 3. Build and start

```bash
docker compose up -d --build
```

- `frontend` (nginx) publishes host port **8080**
- `backend` (.NET) is only reachable inside the compose network as `backend`
- SQLite DB is created and seeded inside the backend container on startup

Check:

```bash
docker compose ps
curl -s localhost:8080 | head
curl -s "localhost:8080/api/products/search?query=a"
```

## 4. Open the port on the LAN

```bash
sudo ufw allow from 192.168.88.0/24 to any port 8080 proto tcp
sudo ufw status
```

(Scope it to the LAN subnet rather than a blanket `allow 8080`.)

## 5. Verify from the Mac

Browser: `http://192.168.88.13:8080`

- SQLi login bypass: username `administrator'--`, any password
- Reflected XSS: search `<img src=x onerror=alert(1)>`

## 6. Point ZAP at it

Target: `http://192.168.88.13:8080`

## Managing the deployment

```bash
docker compose logs -f            # tail logs
docker compose restart            # restart
docker compose down               # stop + remove containers
docker compose up -d --build      # redeploy after a git pull
```
