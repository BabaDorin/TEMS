import * as pulumi from "@pulumi/pulumi";
import * as docker from "@pulumi/docker";

// Keycloak configuration
const keycloakConfig = new pulumi.Config();
const keycloakAdminUser = keycloakConfig.get("adminUser") || "admin";
const keycloakAdminPassword = keycloakConfig.get("adminPassword") || "admin";

// Create a Docker network for TEMS
const temsNetwork = new docker.Network("tems-network", {
    name: "tems-network",
    driver: "bridge",
});

// Pull Keycloak image
const keycloakImage = new docker.RemoteImage("keycloak-image", {
    name: "quay.io/keycloak/keycloak:23.0",
    keepLocally: true,
});

// Create Keycloak container
const keycloakContainer = new docker.Container("keycloak", {
    name: "tems-keycloak",
    image: keycloakImage.imageId,
    restart: "unless-stopped",
    
    envs: [
        "KEYCLOAK_ADMIN=" + keycloakAdminUser,
        "KEYCLOAK_ADMIN_PASSWORD=" + keycloakAdminPassword,
        "KC_HTTP_PORT=8080",
        "KC_HOSTNAME_STRICT=false",
        "KC_HOSTNAME_STRICT_HTTPS=false",
        "KC_HTTP_ENABLED=true",
        "KC_HEALTH_ENABLED=true",
    ],
    
    command: ["start-dev"],
    
    ports: [
        {
            internal: 8080,
            external: 8080,
        },
    ],
    
    networksAdvanced: [
        {
            name: temsNetwork.name,
        },
    ],
    
    healthcheck: {
        tests: ["CMD-SHELL", "exec 3<>/dev/tcp/localhost/8080 && echo -e 'GET /health/ready HTTP/1.1\\r\\nHost: localhost\\r\\n\\r\\n' >&3 && cat <&3 | grep -q '200 OK'"],
        interval: "30s",
        timeout: "10s",
        retries: 5,
        startPeriod: "60s",
    },
}, { dependsOn: [keycloakImage] });

// Export the Keycloak URL
export const keycloakUrl = pulumi.interpolate`http://localhost:8080`;
export const keycloakAdminUrl = pulumi.interpolate`http://localhost:8080/admin`;
export const keycloakAdminUsername = keycloakAdminUser;
export const networkName = temsNetwork.name;
