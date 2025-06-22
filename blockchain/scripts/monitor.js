async function monitor() {
  const contractAddress = "0xbaAA2a3237035A2c7fA2A33c76B44a8C6Fe18e87";
  const Reminders = await ethers.getContractFactory("Reminders");
  const reminders = await Reminders.attach(contractAddress);

  // Listen for events
  reminders.on("ReminderCreated", (id, owner, text, event) => {
    console.log(`✅ Reminder Created: ID=${id}, Owner=${owner}, Text="${text}"`);
  });

  reminders.on("ReminderUpdated", (id, text, event) => {
    console.log(`📝 Reminder Updated: ID=${id}, Text="${text}"`);
  });

  reminders.on("ReminderDeleted", (id, event) => {
    console.log(`🗑️ Reminder Deleted: ID=${id}`);
  });

  console.log("🔍 Monitoring contract events...");
  console.log("Press Ctrl+C to stop");
}

monitor().catch(console.error);