window.reviewDeckLogin = {
    async submit(email, password) {
        const response = await fetch("/account/antiforgery", {
            method: "GET",
            credentials: "same-origin"
        });

        if (!response.ok) {
            throw new Error("Unable to initialize secure login.");
        }

        const { token } = await response.json();

        const form = document.createElement("form");
        form.method = "POST";
        form.action = "/account/login";

        const fields = {
            Email: email,
            Password: password,
            __RequestVerificationToken: token
        };

        for (const [name, value] of Object.entries(fields)) {
            const input = document.createElement("input");
            input.type = "hidden";
            input.name = name;
            input.value = value ?? "";
            form.appendChild(input);
        }

        document.body.appendChild(form);
        form.submit();
    }
};