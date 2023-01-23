import { AzureFunction, Context } from "@azure/functions"

const mongodbTrigger: AzureFunction = async function (context: Context, eventData: string): Promise<void> {
    context.log("MongoDB trigger event data" + JSON.stringify(eventData));
};

export default mongodbTrigger;