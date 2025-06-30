import http from 'k6/http';
import { check, sleep } from 'k6';

const loginUrl = 'http://localhost:9000/api/identity/v1/users/login';
const targetUrl = 'http://localhost:9000/api/consolidations/v1/dailyentries/entries/2025-06-30';

export let options = {
    stages: [
        { duration: '10s', target: 50 }, // Sobe até 50 VUs em 10 segundos
        { duration: '20s', target: 50 }, // Mantém 50 VUs por 20 segundos
        { duration: '10s', target: 0 },  // Finaliza
    ],
    thresholds: {
        http_req_failed: ['rate<0.05'], // Máximo de 5% de falhas permitido
    },
};

let accessToken;

export function setup() {
    const payload = JSON.stringify({
        loginOrEmail: "user",
        password: "OpahIt2025"
    });

    const params = { headers: { 'Content-Type': 'application/json' } };
    let response = http.post(loginUrl, payload, params);

    check(response, {
        'login OK': (r) => r.status === 200,
        'token received': (r) => !!r.json('accessToken'),
    });

    accessToken = response.json('accessToken');
    return { accessToken };
}

export default function (data) {
    let params = {
        headers: {
            'Authorization': `Bearer ${data.accessToken}`
        },
    };

    let res = http.get(targetUrl, params);

    check(res, {
        'status 200': (r) => r.status === 200,
    });

    sleep(0.1); // Pequeno delay para simular comportamento real
}
