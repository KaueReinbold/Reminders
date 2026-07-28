async function monitor() {
  const contractAddress = process.env.BLOCKCHAIN_CONTRACT_ADDRESS || "0xf204a4Ef082f5c04bB89F7D5E6568B796096735a";
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