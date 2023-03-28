package subscriber.common;
 

import java.nio.charset.StandardCharsets;

import com.pcbsys.nirvana.base.nHeader;
import com.pcbsys.nirvana.client.nConsumeEvent;

public class MessageProcessor {

	public void process(nConsumeEvent event) {

		//nHeader header = event.getHeader();
		byte[] eventData = event.getEventData();
		if (eventData != null) {

			String sActualMessage = new String(eventData, StandardCharsets.UTF_8);

			System.out.println("sOutput:" + sActualMessage);
			
			// sActualMessage contains message from Universal messaging topic.
			// Can add relevant code to further process sActualMessage
			

		}
	}
}
