const { ethers } = require("hardhat");

async function main() {
    const Reminders = await ethers.getContractFactory("Reminders");
    const reminders = await Reminders.deploy();

    await reminders.waitForDeployment();

    console.log("Reminders deployed to:", reminders.target);
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
});