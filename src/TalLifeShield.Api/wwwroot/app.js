const e = React.createElement;

function App() {
  const [chatOpen, setChatOpen] = React.useState(false);
  const [messages, setMessages] = React.useState([
    {
      who: "bot",
      text: "Welcome to TAL Life Shield. I can help explain cover options and generate an indicative quote using the knowledge base and backend quote services."
    },
    {
      who: "bot",
      text: "What is happening in your life at the moment?"
    }
  ]);
  const [input, setInput] = React.useState("");
  const [typing, setTyping] = React.useState(false);
  const [quote, setQuote] = React.useState(null);
  const [workflow, setWorkflow] = React.useState([]);
  const [quoteStepIndex, setQuoteStepIndex] = React.useState(null);
  const [quoteAnswers, setQuoteAnswers] = React.useState({});
  const [contactStepIndex, setContactStepIndex] = React.useState(null);
  const [contactAnswers, setContactAnswers] = React.useState({});
  const [quickReplies, setQuickReplies] = React.useState([
    "Recently bought a home",
    "Starting a family",
    "New job or promotion",
    "Reviewing existing cover"
  ]);
  const bodyRef = React.useRef(null);
  const talAgentPhoneNumber = "131 825";

  React.useEffect(() => {
    if (bodyRef.current) {
      bodyRef.current.scrollTop = bodyRef.current.scrollHeight;
    }
  }, [messages, typing, quote, workflow]);

  const fullQuoteState = {
    birthYear: quoteAnswers.birthYear ? Number(quoteAnswers.birthYear) : null,
    gender: quoteAnswers.gender || null,
    isSmoker: quoteAnswers.isSmoker ?? null,
    occupation: quoteAnswers.occupation || null,
    indicativeQuoteShown: Boolean(quote),
    wantsToContinue: false
  };

  const quoteSteps = [
    { key: "isSmoker", question: "Do you smoke?", apiValue: value => value.toLowerCase().startsWith("y") },
    { key: "birthYear", question: "Birth year?", apiValue: value => value.replace(/[^0-9]/g, "").slice(0, 4) },
    { key: "gender", question: "Gender/sex?", apiValue: value => value },
    { key: "occupation", question: "Occupation?", apiValue: value => value }
  ];

  const contactSteps = [
    { key: "name", question: "Full name?" },
    { key: "dateOfBirth", question: "Date of birth?" },
    { key: "postcode", question: "Postcode?" },
    { key: "email", question: "Email address?" },
    { key: "phone", question: "Phone number?" },
    { key: "address", question: "Residential address?" }
  ];

  async function api(path, options) {
    const response = await fetch(path, options);
    if (!response.ok) {
      throw new Error(`Request failed: ${path}`);
    }
    return response.json();
  }

  function addUser(text) {
    setMessages(current => [...current, { who: "user", text }]);
  }

  function addBot(text) {
    setMessages(current => [...current, { who: "bot", text }]);
  }

  async function botDelay(text) {
    setTyping(true);
    await new Promise(resolve => setTimeout(resolve, 450));
    setTyping(false);
    addBot(text);
  }

  function conversationHistory(extraUserText) {
    const turns = extraUserText
      ? [...messages, { who: "user", text: extraUserText }]
      : messages;

    return turns
      .slice(-12)
      .map(message => ({ who: message.who, text: message.text }));
  }

  function firstMissingQuoteStepIndex(answers) {
    return quoteSteps.findIndex(step => {
      const value = answers[step.key];
      return value === undefined || value === null || value === "";
    });
  }

  async function handleQuoteStart(announce = true) {
    if (announce) {
      await botDelay("I can generate an indicative quote using the required non-PII details first.");
    }
    setQuote(null);
    setWorkflow([]);
    setContactAnswers({});
    setContactStepIndex(null);
    const nextIndex = firstMissingQuoteStepIndex(quoteAnswers);
    if (nextIndex === -1) {
      setQuoteStepIndex(null);
      await presentQuickQuote(quoteAnswers);
      return;
    }

    setQuoteStepIndex(nextIndex);
    await botDelay(quoteSteps[nextIndex].question);
  }

  async function handleQuoteAnswer(answer) {
    const currentStep = quoteSteps[quoteStepIndex];
    const nextAnswers = {
      ...quoteAnswers,
      [currentStep.key]: currentStep.apiValue(answer)
    };
    setQuoteAnswers(nextAnswers);

    const nextIndex = firstMissingQuoteStepIndex(nextAnswers);
    if (nextIndex !== -1) {
      setQuoteStepIndex(nextIndex);
      await botDelay(quoteSteps[nextIndex].question);
      return;
    }

    setQuoteStepIndex(null);
    await presentQuickQuote(nextAnswers, answer);
  }

  async function presentQuickQuote(answers, latestUserText) {
    const presentation = await api("/api/chat/quick-quote", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        answers: {
          isSmoker: Boolean(answers.isSmoker),
          birthYear: Number(answers.birthYear),
          gender: answers.gender,
          occupation: answers.occupation
        },
        conversationHistory: conversationHistory(latestUserText)
      })
    });

    setQuote({
      title: presentation.title,
      rows: (presentation.rows || []).map(row => [row.label, row.value]),
      price: presentation.price,
      discounted: Boolean(presentation.discounted)
    });
    await botDelay(presentation.message);
    setQuickReplies(presentation.quickReplies || ["I'm happy to go ahead", "Talk to an agent"]);
  }

  async function handleContactCaptureStart() {
    setContactAnswers({});
    setContactStepIndex(0);
    setQuickReplies([]);
    await botDelay(contactSteps[0].question);
  }

  async function handleContactAnswer(answer) {
    const currentStep = contactSteps[contactStepIndex];
    const nextAnswers = {
      ...contactAnswers,
      [currentStep.key]: answer
    };
    setContactAnswers(nextAnswers);

    const nextIndex = contactStepIndex + 1;
    if (nextIndex < contactSteps.length) {
      setContactStepIndex(nextIndex);
      await botDelay(contactSteps[nextIndex].question);
      return;
    }

    setContactStepIndex(null);
    await botDelay("Thanks. I have captured the contact details needed to continue.");
    setWorkflow(["Quote Generated", "Contact Details Captured"]);
    setQuickReplies(["Continue application", "Talk to an agent"]);
  }

  async function send(text) {
    const trimmed = text.trim();
    if (!trimmed) return;

    addUser(trimmed);
    setInput("");
    setQuickReplies([]);

    if (quoteStepIndex !== null) {
      await handleQuoteAnswer(trimmed);
      return;
    }

    if (contactStepIndex !== null) {
      await handleContactAnswer(trimmed);
      return;
    }

    const response = await api("/api/chat/message", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        message: trimmed,
        state: fullQuoteState,
        conversationHistory: conversationHistory(trimmed)
      })
    });
    await botDelay(response.message);

    if (response.intent === "start_quote") {
      await handleQuoteStart(false);
      return;
    }

    if (response.intent === "collect_contact_details" && quote) {
      await handleContactCaptureStart();
      return;
    }

    setQuickReplies(response.quickReplies || []);
    if (response.quote) {
      setQuote({
        title: response.quote.discountApplied ? "Your Final Indicative Quote" : "Your Indicative Monthly Premium",
        rows: [
          [response.quote.coverDescription, `$${response.quote.sumInsured.toLocaleString()}`],
          ["Quote reference", response.quote.quoteRefNo]
        ],
        price: `$${response.quote.monthlyPremium} / month`,
        discounted: response.quote.discountApplied
      });
    }
    setWorkflow(response.workflowSteps || []);
  }

  function renderQuote() {
    if (!quote) return null;
    return e("div", { className: "quote-card" },
      e("h3", null, quote.title),
      quote.rows.map(row => e("div", { className: "quote-row", key: row[0] },
        e("span", null, row[0]),
        e("strong", null, row[1])
      )),
      e("div", { className: "price" }, quote.price),
      e("div", { className: "fine-print" }, "General information only. This quote is indicative and subject to eligibility and underwriting.")
    );
  }

  function renderWorkflow() {
    if (!workflow.length) return null;
    return e("div", { className: "workflow", "aria-label": "Application workflow status" },
      workflow.map(step => e("div", { className: "workflow-step", key: step }, step))
    );
  }

  function renderChat() {
    return e("aside", { className: "chat", "aria-label": "TAL Life Shield chat assistant" },
      e("div", { className: "chat-head" },
        e("div", { className: "chat-title" },
          e("span", { className: "shield-mark", "aria-hidden": "true" }),
          e("div", null,
            e("div", null, "TAL Life Shield"),
            e("div", { className: "status" }, "AI Quote Assistant")
          )
        ),
        e("button", { className: "icon-button", "aria-label": "Close chat", onClick: () => setChatOpen(false) }, "X")
      ),
      e("div", { className: "chat-body", ref: bodyRef },
        messages.map((message, index) => e("div", { className: `msg ${message.who}`, key: `${message.who}-${index}` }, message.text)),
        typing ? e("div", { className: "msg bot" },
          e("span", { className: "typing", "aria-label": "Assistant is typing" }, e("span"), e("span"), e("span"))
        ) : null,
        renderQuote(),
        renderWorkflow(),
        quickReplies.length ? e("div", { className: "quick-replies" },
          quickReplies.map(reply => e("button", { key: reply, type: "button", onClick: () => send(reply) }, reply))
        ) : null
      ),
      e("form", {
        className: "chat-foot",
        onSubmit: event => {
          event.preventDefault();
          send(input);
        }
      },
        e("div", { className: "chat-input" },
          e("input", {
            value: input,
            onChange: event => setInput(event.target.value),
            placeholder: "Type your question",
            "aria-label": "Message TAL Life Shield"
          }),
          e("button", { type: "submit" }, "Send")
        )
      )
    );
  }

  return e("div", { className: "page" },
    e("div", { className: "top-strip" },
      e("div", { className: "url-pill" }, "https://coverbuilder.tal.com.au/#/partner?sb=WEST&mcode=WESTDEFAULT")
    ),
    e("header", { className: "site-header" },
      e("div", { className: "brand" },
        e("span", { className: "shield-mark", "aria-hidden": "true" }),
        e("span", null, "TAL CoverBuilder")
      ),
      e("nav", { className: "nav", "aria-label": "Primary navigation" },
        e("span", null, "Insurance"),
        e("span", null, "Claims"),
        e("span", null, "Support"),
        e("span", null, "Contact")
      ),
      e("button", { className: "cta", type: "button", onClick: () => setChatOpen(true) }, "Get Started")
    ),
    e("main", { className: "hero" },
      e("section", { className: "hero-copy" },
        e("p", { className: "eyebrow" }, "TAL Life Shield"),
        e("h1", null, "Protect what matters most."),
        e("p", { className: "lead" }, "Explore life insurance options in minutes with a conversational quote assistant that stays grounded in the knowledge base, conversation context, and backend quote APIs."),
        e("div", { className: "hero-actions" },
          e("button", { className: "primary", type: "button", onClick: () => { setChatOpen(true); send("Start a quick quote"); } }, "Start Your Quote"),
          e("button", { className: "secondary", type: "button", onClick: () => { setChatOpen(true); send("Show me options"); } }, "Learn More")
        )
      ),
      e("section", { className: "hero-image", "aria-label": "Family at home representing protection" })
    ),
    chatOpen ? renderChat() : e("button", { className: "chat-launch", "aria-label": "Open TAL Life Shield chat", onClick: () => setChatOpen(true) }, "Chat")
  );
}

ReactDOM.createRoot(document.getElementById("root")).render(e(App));
