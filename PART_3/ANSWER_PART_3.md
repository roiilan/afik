# How to Run 

To run it, you need to run the backend and the frontend.

Backend - open a terminal in the project root and execute:

```bash
cd ./PART_3/react_proj/backend
npm run dev
```

Frontend - open a new terminal in the project root and execute:

```bash
cd ./PART_3/react_proj/frontend
npm run dev
```

And then open in your browser: http://127.0.0.1:5173/

# Questions and Answer

## 1. If we had to move the calculation logic to a resource-constrained Embedded component (C++/Assembly), what memory considerations would you take into account?

I would mainly focus on reducing memory usage and keeping the code predictable and simple.

1) I would avoid dynamic memory allocations as much as possible in order to prevent fragmentation and long-term stability issues, and I would prefer fixed data structures such as `struct` and fixed-size buffers.

2) I would not send or store the entire object if not all of its fields are truly required for the calculation. For example, if the decision logic only uses `temperature`, `current`, `voltage`, and `status`, then there is no need to pass `id` or other fields that do not participate in the rules. This reduces both memory usage and communication/processing overhead.

3) The data representation. Instead of using `double` for every value, I would check whether it is possible to use `float` or even scaled integers, in order to save memory and simplify calculations on weaker hardware.

4) I would not necessarily return long text strings from the embedded component itself. Instead, I would consider returning an `enum` or a short fault code, and only in an outer layer translate it into user-friendly text. This can significantly reduce memory usage and preserve a good separation between the calculation logic and the presentation layer.

5) If trend analysis or historical analysis is needed in the future, I would not keep a large number of raw samples. Instead, I would use a small ring buffer or incremental calculations such as moving average, min/max, or counters.

Overall, my goal in an Embedded environment is to stay minimal:
- keep only what is necessary
- use compact representations
- avoid unnecessary allocations
- and return a small, deterministic result

## 2. How would you secure the API endpoint that exposes device data?

I would take a layered security approach rather than relying only on basic access checks.

1) I would require user or system authentication using a mechanism such as JWT, OAuth2, or an internal API key, depending on the type of system and the API consumers.

2) I would add authorization checks to ensure that not every authenticated user can view all devices, but only the data they are allowed to access according to role, customer, site, or relevant device group.

3) I would enforce HTTPS for all communication in order to prevent exposure of data or tokens in transit.

4) I would validate and sanitize all input reaching the endpoint, even if it is only simple parameters, in order to prevent injections, misuse of data, or unexpected requests.

5) If there are database queries, I would use parameterized queries or a safe ORM to prevent SQL injection.

6) To reduce the risk of abuse, I would also add rate limiting and throttling in order to prevent scraping, brute force, or abnormal load on the system.

7) I would implement audit logging for access to device data, so it would be possible to know who accessed which data, when, and from which source. This is important both for security and for troubleshooting.

8) From an information exposure perspective, I would follow the principle of least privilege — meaning I would return only the fields that are actually required by the consumer, and would not expose extra information such as internal identifiers, unnecessary metadata, or details that are not needed for the operation.

For example, if the AI logic only needs `temperature`, `current`, `voltage`, and `status`, then there is no reason to send additional fields such as internal identifiers if they are not required.

9) if the endpoint is especially sensitive or internal, I would also consider additional layers such as IP allowlisting, separation between internal and external networks, tightly restricted CORS, and proper secret management through environment variables or a secret manager.

In other words, the goal is not just to “lock down the API,” but to build an endpoint that is authenticated, restricted, monitored, and returns only the required information in a controlled way.

## 3. What questions would you ask the Product Manager before starting development of the AI Insights component?

Before starting development of the AI Insights component, I would ask several key questions in order to understand exactly what the component is supposed to do, who it is intended for, and how it should behave.

1. What is the purpose of the AI Insights component? 
I would ask whether the goal is only to explain why a device was marked as anomalous, or also to help the user understand how severe the situation is and what should be checked.  
I would ask this because the purpose of the component directly affects how it should be implemented. If the goal is only a short explanation, a simple text may be enough. If the goal is also to support decision-making, the explanation needs to be more useful and precise. There is no point in returning long text because it complicates the user experience and increases costs.

2. Who are the main users of the component? 
I would ask whether the users are operations staff, technical users, support representatives, or business managers.  
I would ask this because the type of user affects how the insight should be written. A technical user will want more professional detail, while a business user will prefer a short, clear, and simple explanation.

3. Should the insight be general, or should it also explain which data led to the conclusion? 
I would ask whether it is enough to return a sentence like “Possible overload,” or whether it should also explain that the conclusion is based, for example, on high temperature and abnormal current.  
I would ask this because the level of detail affects how well the user understands the result and how much they trust it. The more transparent and explained the insight is, the easier it is to understand and rely on it.

4. What should happen if a clear insight cannot be generated? 
I would ask what should be shown if there is not enough data, if the service fails, or if no clear reason for the anomaly is found.  
I would ask this because it is important to define in advance how the system behaves in failure cases, so that the experience remains consistent and does not appear broken or confusing.

Ultimately, the goal of these questions is to ensure that development of the AI Insights component is aligned with the real product need, and does not simply add an unnecessary layer of text to the screen.



# Project Logic Overview

When the Dashboard component loads, it imports mock device data from the file **exceptionalDevices.js** and displays it in a table using **ag-grid-react**.
Each row represents a device with the fields: `id`, `device_id`, `timestamp`, `voltage`, `current`, `temperature`, and `status`.

The table includes an **AI Insight** column with a **"Generate AI Insights"** button per row.
When the button is clicked, a **GET** request is sent to the backend endpoint `/api/ai-insight`, passing the device's `voltage`, `current`, `temperature`, and `status` as query parameters.

The backend receives the request, validates the required fields, and calls the `generateInsight` function.
This function uses a **switch case** statement that evaluates combinations of the received values — such as high temperature with high current, critical status with low voltage, and so on — to match a relevant rule and return a textual explanation describing the likely cause of the anomaly.

The response (`{ insight: "..." }`) is sent back to the frontend and displayed in the corresponding row.
If the request fails, a "Bad Connection" error message is shown temporarily, and after 7 seconds it disappears and the "Generate AI Insights" button appears.