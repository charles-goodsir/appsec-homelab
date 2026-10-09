# Deploying to the mini PC (DAST target)

Goal: run the vulnerable app on the homelab mini PC so OWASP ZAP can scan it
over the LAN. The app is **only** ever exposed on the home network.

Mini PC: Linux Mint, user `owner`, `192.168.88.18` (Wi-Fi).

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

- `frontend` (nginx) publishes host port **8080** on 127.0.0.1 only
- `backend` (.NET) is only reachable inside the compose network as `backend`
- SQLite DB is created and seeded inside the backend container on startup

Check:

```bash
docker compose ps
curl -s localhost:8080 | head
curl -s "localhost:8080/api/products/search?query=a"
```

## 4. Reach it from the Mac (SSH tunnel)

The app is deliberately vulnerable, so port 8080 is bound to loopback and is
not exposed on the LAN. Docker-published ports bypass `ufw allow` rules, so
binding to 127.0.0.1 is the control, not the firewall. Tunnel in instead:

```bash
ssh -N -L 8080:localhost:8080 owner@192.168.88.18
```

## 5. Verify from the Mac

With the tunnel open, browse to `http://localhost:8080`

- SQLi login bypass: username `administrator'--`, any password
- Reflected XSS: search `<img src=x onerror=alert(1)>`

Check the port is closed on the LAN (expect a failure):

```bash
nc -z -G 3 192.168.88.18 8080
```

## 6. Point ZAP at it

Target: `http://localhost:8080` (through the tunnel)

## Managing the deployment

```bash
docker compose logs -f            # tail logs
docker compose restart            # restart
docker compose down               # stop + remove containers
docker compose up -d --build      # redeploy after a git pull
```
