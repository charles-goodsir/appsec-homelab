# Infrastructure as Code: Azure resource group + storage account

Goal: provision a small, real piece of cloud infrastructure declaratively
with Terraform, as a companion to the manual/imperative setup used
elsewhere in this repo (see [DEPLOY.md](../../DEPLOY.md)).

## Before: manual / imperative

Everything else in this repo is stood up by hand, one command at a time:

```bash
git clone https://github.com/charles-goodsir/appsec-homelab.git
cd appsec-homelab
docker compose up -d --build
```

This works, but it has the usual downsides of imperative setup:

- The *current* state of the environment only exists in whoever's shell
  history ran the commands — there's no single source of truth for what
  was actually created.
- Nothing shows you what would change before you run it. `docker compose up`
  just does it.
- Repeating it exactly (e.g. on a second machine, or after a mistake) means
  re-running the same manual steps and hoping nothing was missed.
- There's no built-in way to tear the environment down cleanly — you'd have
  to remember and reverse each step yourself.

## After: declarative with Terraform

This directory (`infra/azure-storage/`) instead *describes* the desired
end state in `.tf` files, and lets Terraform work out how to get there:

- [`providers.tf`](providers.tf) — configures the `azurerm` provider,
  authenticating via the local `az login` session (no credentials stored
  in the repo).
- [`resource_group.tf`](resource_group.tf) — an `azurerm_resource_group`
  (`rg-appsec-homelab`, `australiaeast`).
- [`storage_account.tf`](storage_account.tf) — an `azurerm_storage_account`
  (`Standard`/`LRS`) created inside that resource group.

Workflow:

```bash
terraform init    # download the azurerm provider plugin
terraform plan    # preview exactly what will be created/changed/destroyed
terraform apply   # create it for real, after confirming
```

`terraform plan` is the key difference from the Docker Compose approach —
it's a dry run that shows every resource and attribute that will change
*before* anything is touched, so there are no surprises going into `apply`.

The `.terraform.lock.hcl` file is committed (it pins exact provider
versions for reproducibility); the `.terraform/` plugin cache and any
`*.tfstate*` files are gitignored, since they're machine-specific/local
state rather than something to version.

## Teardown

Since this is a scoped exercise rather than infrastructure the homelab
depends on, it's torn down once documented:

```bash
terraform destroy
```

This removes both resources from Azure and avoids any ongoing cost.
